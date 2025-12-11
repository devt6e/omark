using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureRotateGizmo : MonoBehaviour
{
    public static FurnitureRotateGizmo Instance {get; private set;}
    [Header("Handle")]
    public Transform rotateRing;      // 회전 링(콜라이더 포함)

    [Header("Raycast")]
    public LayerMask gizmoMask;
    public float maxRayDistance = 100f;

    private Camera cam;
    private FurniturePiece target;

    private bool isDragging = false;

    private Vector3 dragStartDir;     // 중심→포인터 방향(시작)
    private float startRotationY;     // 시작 Y 회전값

    public void SetCamera(Camera newCam) => cam = newCam;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (target == null)
            return;

        if (FurnitureGizmoController.Instance.CurrentMode != GizmoMode.Rotate)
            return;

        // 기즈모 위치/회전: 가구 중심 + world up 기준
        transform.position = target.Pivot.position;
        transform.rotation = Quaternion.identity;

        if (IsPointerOverUI())
            return;

        var input = InputReader.Instance;

        if (!isDragging && input.ContactStarted)
        {
            TryBeginDrag(input.Point);
        }
        else if (isDragging && input.ContactActive)
        {
            Drag(input.Point);
        }
        else if (isDragging && input.ContactEnded)
        {
            EndDrag();
        }
    }

    public void AttachTarget(FurniturePiece piece)
    {
        target = piece;
        isDragging = false;
        GizmoInputBlocker.IsDraggingGizmo = false;

        if (target != null && FurnitureGizmoController.Instance.CurrentMode == GizmoMode.Rotate)
        {
            transform.position = target.Pivot.position;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Detach()
    {
        target = null;
        isDragging = false;
        GizmoInputBlocker.IsDraggingGizmo = false;
        gameObject.SetActive(false);
    }

    private void TryBeginDrag(Vector2 screenPos)
    {
        if (target == null)
            return;

        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, gizmoMask))
        {
            if (!IsRotateHandle(hit.transform))
                return;

            GizmoInputBlocker.IsDraggingGizmo = true;
            isDragging = true;

            Vector3 center = target.Pivot.position;
            Vector3 startPoint = GetPointOnHorizontalPlane(screenPos, center.y);

            dragStartDir = (startPoint - center);
            dragStartDir.y = 0f;
            dragStartDir.Normalize();

            startRotationY = target.transform.eulerAngles.y;
        }
    }

    private void Drag(Vector2 screenPos)
    {
        if (target == null || !isDragging)
            return;

        Vector3 center = target.Pivot.position;
        Vector3 currentPoint = GetPointOnHorizontalPlane(screenPos, center.y);

        Vector3 currentDir = (currentPoint - center);
        currentDir.y = 0f;
        currentDir.Normalize();

        if (currentDir.sqrMagnitude < 0.0001f || dragStartDir.sqrMagnitude < 0.0001f)
            return;

        float angle = Vector3.SignedAngle(dragStartDir, currentDir, Vector3.up);

        Vector3 euler = target.transform.eulerAngles;
        euler.y = startRotationY + angle;
        target.transform.eulerAngles = euler;
    }

    private void EndDrag()
    {
        isDragging = false;
        GizmoInputBlocker.IsDraggingGizmo = false;
    }

    private bool IsRotateHandle(Transform t)
    {
        if (t == rotateRing) return true;
        if (t.parent != null)
            return IsRotateHandle(t.parent);
        return false;
    }

    private Vector3 GetPointOnHorizontalPlane(Vector2 screenPos, float y)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, new Vector3(0, y, 0));

        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return new Vector3(0, y, 0);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#else
        if (UnityEngine.Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId);
        return false;
#endif
    }
}
