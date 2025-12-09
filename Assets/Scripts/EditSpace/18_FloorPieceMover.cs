using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FloorPieceMover : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference pointAction;
    public InputActionReference contactAction;

    [Header("Settings")]
    public float dragPlaneHeight = 0f; // 바닥 y=0
    public float snapThreshold = 0.1f; // SnapUtil에서 사용되는 스냅 반경

    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragOffset;

    private FloorPiece targetPiece; // 현재 이동 중인 바닥
    private Plane dragPlane;

    private void Awake()
    {
        cam = Camera.main;
        dragPlane = new Plane(Vector3.up, new Vector3(0, dragPlaneHeight, 0));
    }

    private void OnEnable()
    {
        pointAction.action.Enable();
        contactAction.action.Enable();

        contactAction.action.started += OnPressStarted;
        contactAction.action.performed += OnPressHeld;
        contactAction.action.canceled += OnPressCanceled;
    }

    private void OnDisable()
    {
        pointAction.action.Disable();
        contactAction.action.Disable();

        contactAction.action.started -= OnPressStarted;
        contactAction.action.performed -= OnPressHeld;
        contactAction.action.canceled -= OnPressCanceled;
    }

    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.EditFloor)
            return;

        if (IsPointerOverUI())
            return;

        // 현재 선택된 FloorPiece 1개만 이동 가능
        var selection = SelectionManagerAccessor.GetSelection();
        if (selection == null || selection.Count != 1)
            return;

        targetPiece = selection[0];
        if (targetPiece == null) return;

        // 드래그 시작점 계산
        Vector3 hitPoint = GetPointerWorldPos();

        dragOffset = targetPiece.transform.position - hitPoint;
        isDragging = true;
    }

    private void OnPressHeld(InputAction.CallbackContext ctx)
    {
        if (!isDragging || targetPiece == null)
            return;

        Vector3 hitPoint = GetPointerWorldPos();
        Vector3 newPos = hitPoint + dragOffset;

        // SnapUtil 적용
        newPos = SnapUtil.SnapFloorPiecePosition(newPos, targetPiece, snapThreshold);

        // 실제 위치 적용
        targetPiece.transform.position = newPos;
        targetPiece.ShowSizeUI(); // 이동간 UI 동기화
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        isDragging = false;
        targetPiece = null;
    }

    // ============================================================
    // Utility
    // ============================================================
    private Vector3 GetPointerWorldPos()
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (dragPlane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
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
