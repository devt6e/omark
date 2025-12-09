using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference pointAction;
    public InputActionReference contactAction;

    [Header("Settings")]
    public float longPressTime = 1.0f;

    [Header("UI")]
    public Canvas uiCanvas;
    public RectTransform deleteButton;
    public float groupButtonYOffset = 150f; // 전체 선택 시 화면 하단에서 조금 위
    public SizeUIController sizeUIController;

    private Camera cam;
    private float pressStartTime;
    private bool isPressing = false;

    private FloorPiece pressedPiece;

    private List<FloorPiece> currentSelection = new();

    private void Awake()
    {
        cam = Camera.main;
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
        if (EditorModeManager.Instance.CurrentMode != EditMode.MoveView2D &&
            EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
            return;

        if (isPressing && pressedPiece != null)
        {
            float holdTime = Time.time - pressStartTime;

            if (holdTime >= longPressTime)
            {
                SelectAll();
                isPressing = false;
            }
        }
    }

    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.MoveView2D &&
        EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
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

    // ======================================
    // 선택 관련
    // ======================================
    private void SelectSingle(FloorPiece piece)
    {
        ClearSelection();
        currentSelection.Add(piece);
        piece.Select();

        UpdateDeleteButtonPosition();
        if (EditorModeManager.Instance.CurrentMode == EditMode.EditFloor)
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
        foreach (var piece in currentSelection)
        {
            piece.Deselect();
            piece.HideSizeUI();
        }

        currentSelection.Clear();

        UpdateDeleteButtonPosition();
    }

    // ======================================
    // 삭제 버튼 위치 갱신
    // ======================================
    private void UpdateDeleteButtonPosition()
    {
        if (deleteButton == null || uiCanvas == null)
            return;

        // 선택이 아무 것도 없으면 버튼 숨기기
        if (currentSelection.Count == 0)
        {
            deleteButton.gameObject.SetActive(false);
            return;
        }

        // 1개 선택: 해당 바닥 중앙에 버튼 표시
        if (currentSelection.Count == 1)
        {
            FloorPiece piece = currentSelection[0];
            if (piece == null)
            {
                deleteButton.gameObject.SetActive(false);
                return;
            }

            // anchor 복원 (중요!)
            var rt = deleteButton;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot    = new Vector2(0.5f, 0.5f);

            Vector3 worldPos = piece.GetBounds().center;
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0)
            {
                deleteButton.gameObject.SetActive(false);
                return;
            }

            RectTransform canvasRect = uiCanvas.transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera,
                out Vector2 localPos
            );

            deleteButton.gameObject.SetActive(true);
            deleteButton.anchoredPosition = localPos;
        }
        // 여러 개 선택 (롱프레스 전체 선택): 화면 하단 중앙 근처에 표시
        else
        {
            deleteButton.gameObject.SetActive(true);

            RectTransform canvasRect = uiCanvas.transform as RectTransform;
            var rt = deleteButton;

            // 앵커를 하단 중앙으로
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot    = new Vector2(0.5f, 0.5f);

            // 하단에서 groupButtonYOffset 만큼 위
            rt.anchoredPosition = new Vector2(0f, groupButtonYOffset);
        }
    }
    // ======================================
    // 삭제 기능
    // ======================================
    public void OnClickDeleteButton()
    {
        if (currentSelection.Count == 0)
            return;

        // ============================
        // 1개 선택 → 단일 삭제
        // ============================
        if (currentSelection.Count == 1)
        {
            FloorPiece target = currentSelection[0];

            // // RoomManager 중간 바닥 검사
            // if (RoomManager.Instance.IsMiddlePiece(target))
            // {
            //     Debug.Log("<color=yellow>중간 FloorPiece는 삭제할 수 없습니다.</color>");
            //     ClearSelection();
            //     return;
            // }

            RoomManager.Instance.DeletePiece(target);
            ClearSelection();
            return;
        }

        // ============================
        // 여러 개 선택 → 그룹 삭제
        // ============================
        foreach (var piece in currentSelection)
        {
            if (piece == null) continue;

            // 그룹 삭제 시에는 중간 바닥 개념 삭제 X
            // 전체가 함께 삭제되므로 그래프 무결성 문제 없음
            RoomManager.Instance.DeletePiece(piece);
        }

        ClearSelection();
    }


    // ======================================
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

        if (Mouse.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Touchscreen.current != null)
        {
            foreach (var t in Touchscreen.current.touches)
            {
                if (t.isInProgress)
                    if (EventSystem.current.IsPointerOverGameObject(t.touchId.ReadValue()))
                        return true;
            }
        }

        return false;
    }
}
