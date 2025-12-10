using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    [Header("Input Actions")]
    public InputActionReference pointAction;
    public InputActionReference contactAction;

    [Header("Settings")]
    public float longPressTime = 1.0f;

    [Header("UI")]
    public Canvas uiCanvas;
    public RectTransform deleteButton;
    public RectTransform resizeButton;     // ★ Resize 버튼
    public float groupButtonYOffset = 150f;

    private Camera cam;
    private float pressStartTime;
    private bool isPressing = false;

    private FloorPiece pressedPiece;
    private List<FloorPiece> currentSelection = new();
    public List<FloorPiece> GetCurrentSelection() => currentSelection;

    private void Awake()
    {
        cam = Camera.main;
        Instance = this;

        deleteButton.gameObject.SetActive(false);
        resizeButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        pointAction.action.Enable();
        contactAction.action.Enable();

        contactAction.action.started += OnPressStarted;
        contactAction.action.canceled += OnPressCanceled;
    }

    private void OnDisable()
    {
        pointAction.action.Disable();
        contactAction.action.Disable();

        contactAction.action.started -= OnPressStarted;
        contactAction.action.canceled -= OnPressCanceled;
    }

    private void Update()
    {
        // EditFloor 모드에서만 동작
        if (EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
            return;

        if (isPressing && pressedPiece != null)
        {
            float hold = Time.time - pressStartTime;
            if (hold >= longPressTime)
            {
                SelectAll();
                isPressing = false;
            }
        }
    }

    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
            return;

        if (IsPointerOverUI())
            return;

        pressStartTime = Time.time;
        isPressing = true;
        pressedPiece = RaycastFloorPiece();
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        if (!isPressing) return;
        isPressing = false;

        float held = Time.time - pressStartTime;

        if (held >= longPressTime)
            return;

        if (pressedPiece == null)
        {
            ClearSelection();
            return;
        }

        if (currentSelection.Count == 1 && currentSelection.Contains(pressedPiece))
        {
            ClearSelection();
            return;
        }

        SelectSingle(pressedPiece);
    }

    // ============================================================
    // 선택 처리
    // ============================================================
    private void SelectSingle(FloorPiece piece)
    {
        Debug.Log("Actual Label active? " + DimensionLabelUIManager.Instance.widthActualLabel.gameObject.activeSelf);

        ClearSelection();

        currentSelection.Add(piece);
        piece.Select();

        UpdateActionButtons();

        ShowDimensionForPiece(piece);
    }

    private void SelectAll()
    {
        ClearSelection();

        foreach (var piece in RoomManager.Instance.GetAllPieces())
        {
            currentSelection.Add(piece);
            piece.Select();
        }

        UpdateActionButtons();
    }

    public void ClearSelection()
    {
        foreach (var p in currentSelection)
            p.Deselect();

        currentSelection.Clear();
        
        DimensionLabelUIManager.Instance.HideActual();
        UpdateActionButtons();
    }

    // ============================================================
    // 버튼 표시/숨김 & 배치
    // ============================================================
    private void UpdateActionButtons()
    {
        // 모드 체크
        if (EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
        {
            deleteButton.gameObject.SetActive(false);
            resizeButton.gameObject.SetActive(false);
            return;
        }

        // Resize Popup이 떠있으면 두 버튼 모두 숨김
        if (ResizePopupUI.Instance != null &&
            ResizePopupUI.Instance.popupRoot.activeSelf)
        {
            deleteButton.gameObject.SetActive(false);
            resizeButton.gameObject.SetActive(false);
            return;
        }

        // 선택 없음
        if (currentSelection.Count == 0)
        {
            deleteButton.gameObject.SetActive(false);
            resizeButton.gameObject.SetActive(false);
            
            return;
        }

        // 여러 개 선택 → Delete만 표시, Resize 숨김
        if (currentSelection.Count > 1)
        {
            deleteButton.gameObject.SetActive(true);
            resizeButton.gameObject.SetActive(false);

            PositionButtonsForMultiple();
            return;
        }

        // 1개 선택 → Delete & Resize 둘 다 ON
        deleteButton.gameObject.SetActive(true);
        resizeButton.gameObject.SetActive(true);

        PositionButtonsForSingle();
    }

    private void PositionButtonsForMultiple()
    {
        var rt = deleteButton;
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, groupButtonYOffset);
    }

    private void PositionButtonsForSingle()
    {
        // Delete 왼쪽
        var d = deleteButton;
        d.anchorMin = new Vector2(0.5f, 0f);
        d.anchorMax = new Vector2(0.5f, 0f);
        d.pivot     = new Vector2(0.5f, 0.5f);
        d.anchoredPosition = new Vector2(-80, groupButtonYOffset);

        // Resize 오른쪽
        var r = resizeButton;
        r.anchorMin = new Vector2(0.5f, 0f);
        r.anchorMax = new Vector2(0.5f, 0f);
        r.pivot     = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = new Vector2(80, groupButtonYOffset);
    }

    // ============================================================
    // 삭제
    // ============================================================
    public void OnClickDeleteButton()
    {
        if (currentSelection.Count == 0) return;

        foreach (var p in currentSelection)
            if (p != null)
                RoomManager.Instance.DeletePiece(p);

        ClearSelection();
    }

    // ============================================================
    // 리사이즈 버튼 → 팝업 열기
    // ============================================================
    public void OnClickResizeButton()
    {
        if (currentSelection.Count != 1) return;

        FloorPiece p = currentSelection[0];
        ResizePopupUI.Instance.Show(p);

        DimensionLabelUIManager.Instance.HideActual();
        UpdateActionButtons();
    }

    // ============================================================
    // 유틸리티
    // ============================================================
    private FloorPiece RaycastFloorPiece()
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            return hit.collider.GetComponent<FloorPiece>();

        return null;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // 마우스
        if (Mouse.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return true;

        // 터치
        if (Touchscreen.current != null)
        {
            foreach (var t in Touchscreen.current.touches)
            {
                if (t.isInProgress &&
                    EventSystem.current.IsPointerOverGameObject(t.touchId.ReadValue()))
                    return true;
                }
        }
        return false;
    }

    private void ShowDimensionForPiece(FloorPiece piece)
    {
        var mgr = DimensionLabelUIManager.Instance;
        Bounds b = piece.GetBounds();

        // Width label (위쪽)
        mgr.widthActualLabel.placeAbove = true;
        mgr.widthActualLabel.placeRight = false;
        mgr.widthActualLabel.SetWorldLabel(
            new Vector3(b.center.x, 0, b.max.z),
            b.size.x
        );

        // Height label (오른쪽)
        mgr.heightActualLabel.placeAbove = false;
        mgr.heightActualLabel.placeRight = true;
        mgr.heightActualLabel.SetWorldLabel(
            new Vector3(b.max.x, 0, b.center.z),
            b.size.z
        );
    }
}
