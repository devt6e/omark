using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FloorPieceMover : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference pointAction;        // Pointer position
    public InputActionReference contactAction;      // Pointer press

    [Header("Settings")]
    public float dragPlaneHeight = 0f;  // 바닥 y=0
    public float snapThreshold = 0.15f;  // SnapUtil에서 사용되는 스냅 반경

    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragOffset;

    private FloorPiece targetPiece; 
    private Plane dragPlane;

    private SnapFeedbackRenderer snapFX;

    private void Awake()
    {
        cam = Camera.main;
        dragPlane = new Plane(Vector3.up, new Vector3(0, dragPlaneHeight, 0));
        snapFX = FindAnyObjectByType<SnapFeedbackRenderer>();
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

    // ============================================================
    // 드래그 시작
    // ============================================================
    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        // 이동은 EditFloor 모드에서만 가능
        if (EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
            return;

        if (IsPointerOverUI())
            return;

        // 카메라 재확인
        cam = Camera.main;

        // 선택된 FloorPiece 1개만 이동 가능
        List<FloorPiece> selection = SelectionManagerAccessor.GetSelection();
        if (selection == null || selection.Count != 1)
            return;

        targetPiece = selection[0];
        if (targetPiece == null) return;

        // 드래그 시작 지점 계산
        Vector3 hitPoint = GetPointerWorldPos();
        dragOffset = targetPiece.transform.position - hitPoint;

        isDragging = true;
    }

    // ============================================================
    // 드래그 종료
    // ============================================================
    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        isDragging = false;
        targetPiece = null;
    }

    // ============================================================
    // 이동 로직 (매 프레임)
    // ============================================================
    private void Update()
    {
        if (!isDragging || targetPiece == null)
        {
            if (snapFX != null)
                snapFX.Hide();
            return;
        }

        // Raw movement
        Vector3 hitPoint = GetPointerWorldPos();
        Vector3 newPosRaw = hitPoint + dragOffset;

        // Get full snap info
        SnapResult sr = SnapUtil.GetSnapResult(newPosRaw, targetPiece, snapThreshold);

        // Apply position
        targetPiece.transform.position = sr.snappedPos;
        // FloorDrawer.Instance.UpdateDimensionPreview(
        //     targetPiece.transform.position,
        //     targetPiece.transform.localScale.x, 
        //     targetPiece.transform.localScale.z);
    }

    // ============================================================
    // 유틸: 화면 좌표 → 월드 좌표
    // ============================================================
    private Vector3 GetPointerWorldPos()
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (dragPlane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }

    // ============================================================
    // 유틸: UI 충돌 체크
    // ============================================================
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) 
            return false;

        // 마우스
        if (Mouse.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        // 모바일 터치
        if (Touchscreen.current != null)
        {
            foreach (var t in Touchscreen.current.touches)
                if (t.isInProgress && EventSystem.current.IsPointerOverGameObject(t.touchId.ReadValue()))
                    return true;
        }
        return false;
    }
}
