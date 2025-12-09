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
    public float groupButtonYOffset = 150f;
    public SizeUIController sizeUIController;

    public List<FloorPiece> GetCurrentSelection() => currentSelection;

    private Camera cam;
    private float pressStartTime;
    private bool isPressing = false;

    private FloorPiece pressedPiece;
    private List<FloorPiece> currentSelection = new();

    private void Awake()
    {
        cam = Camera.main;
        Instance = this;
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
        // ✔ EditFloor 모드에서만 선택/롱프레스 작동
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
        // ✔ EditFloor 모드에서만 터치 처리
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
        if (!isPressing)
            return;

        isPressing = false;

        // 롱프레스는 이미 처리됨
        float held = Time.time - pressStartTime;
        if (held >= longPressTime)
            return;

        // 단일 터치 제스처
        if (pressedPiece == null)
        {
            ClearSelection();
            return;
        }

        // 이미 선택된 걸 다시 누르면 선택 해제
        if (currentSelection.Count == 1 && currentSelection.Contains(pressedPiece))
        {
            ClearSelection();
            return;
        }

        SelectSingle(pressedPiece);
    }

    // ============================
    // 선택 처리
    // ============================
    private void SelectSingle(FloorPiece piece)
    {
        ClearSelection();

        currentSelection.Add(piece);
        piece.Select();

        UpdateDeleteButtonPosition();

        // EditFloor 모드에서만 크기 UI 표시
        piece.ShowSizeUI();
    }

    private void SelectAll()
    {
        ClearSelection();

        foreach (var piece in RoomManager.Instance.GetAllPieces())
        {
            currentSelection.Add(piece);
            piece.Select();
        }

        UpdateDeleteButtonPosition();
    }

    private void ClearSelection()
    {
        foreach (var p in currentSelection)
        {
            p.Deselect();
            p.HideSizeUI();
        }

        currentSelection.Clear();
        UpdateDeleteButtonPosition();
    }

    // ============================
    // 삭제 버튼 위치 처리
    // ============================
    private void UpdateDeleteButtonPosition()
    {
        // EditFloor 모드가 아니라면 숨김
        if (EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
        {
            deleteButton.gameObject.SetActive(false);
            return;
        }

        if (currentSelection.Count == 0)
        {
            deleteButton.gameObject.SetActive(false);
            return;
        }

        // 여러 개 선택 시 하단 중앙 고정
        if (currentSelection.Count > 1)
        {
            deleteButton.gameObject.SetActive(true);

            var rt = deleteButton;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, groupButtonYOffset);
            return;
        }

        // 단일 선택 시 하단 중앙
        {
            deleteButton.gameObject.SetActive(true);

            var rt = deleteButton;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, groupButtonYOffset);
        }
    }

    // ============================
    // 삭제 버튼 클릭
    // ============================
    public void OnClickDeleteButton()
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
            return;

        if (currentSelection.Count == 0)
            return;

        foreach (var p in currentSelection)
        {
            if (p != null)
                RoomManager.Instance.DeletePiece(p);
        }

        ClearSelection();
    }

    // ============================
    // 유틸리티
    // ============================
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
        if (EventSystem.current == null)
            return false;

        if (Mouse.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Touchscreen.current != null)
        {
            foreach (var t in Touchscreen.current.touches)
            {
                if (t.isInProgress)
                {
                    if (EventSystem.current.IsPointerOverGameObject(t.touchId.ReadValue()))
                        return true;
                }
            }
        }
        return false;
    }
}
