using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureMoveGizmo : MonoBehaviour
{
    public static FurnitureMoveGizmo Instance { get; private set; }

    [Header("Handles")]
    public Transform axisX;
    public Transform axisY;
    public Transform axisZ;

    [Header("Raycast")]
    public LayerMask gizmoMask;
    public float maxRayDistance = 100f;

    private Camera cam;
    private FurniturePiece target;

    private enum HandleType { None, AxisX, AxisY, AxisZ }
    private HandleType activeHandle = HandleType.None;

    private Vector3 dragStartTargetPos;
    private Vector3 dragStartHitPoint;
    private Vector3 activeAxisDir;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        cam = GlobalCameraManager.Camera3D;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (target == null)
            return;

        // 기즈모는 항상 타겟 위치를 따라간다
        transform.position = target.Pivot.position;
        transform.rotation = Quaternion.identity;

        if (IsPointerOverUI())
            return;

        // 1) 드래그 시작
        if (InputReader.Instance.ContactStarted)
        {
            TryBeginDrag(InputReader.Instance.Point);
        }
        // 2) 드래그 중
        else if (InputReader.Instance.ContactActive && activeHandle != HandleType.None)
        {
            Drag(InputReader.Instance.Point);
        }
        // 3) 드래그 종료
        else if (InputReader.Instance.ContactEnded && activeHandle != HandleType.None)
        {
            EndDrag();
        }
    }

    public void AttachTo(FurniturePiece piece)
    {
        target = piece;
        activeHandle = HandleType.None;
        GizmoInputBlocker.IsDraggingGizmo = false;

        if (target != null)
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
        activeHandle = HandleType.None;
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
            HandleType handle = GetHandleType(hit.transform);
            if (handle == HandleType.None)
                return;

            activeHandle = handle;
            GizmoInputBlocker.IsDraggingGizmo = true;

            dragStartTargetPos = target.transform.position;
            dragStartHitPoint = GetWorldPoint(screenPos, dragStartTargetPos);

            switch (activeHandle)
            {
                case HandleType.AxisX: activeAxisDir = Vector3.right; break;
                case HandleType.AxisY: activeAxisDir = Vector3.up; break;
                case HandleType.AxisZ: activeAxisDir = Vector3.forward; break;
            }
        }
    }

    private void Drag(Vector2 screenPos)
    {
        if (target == null || activeHandle == HandleType.None)
            return;

        Vector3 currentHit = GetWorldPoint(screenPos, dragStartTargetPos);
        Vector3 delta = currentHit - dragStartHitPoint;

        Vector3 projectedDelta = Vector3.Project(delta, activeAxisDir);
        Vector3 newPos = dragStartTargetPos + projectedDelta;

        target.transform.position = newPos;
    }

    private void EndDrag()
    {
        activeHandle = HandleType.None;
        GizmoInputBlocker.IsDraggingGizmo = false;
    }

    private HandleType GetHandleType(Transform h)
    {
        if (h == axisX) return HandleType.AxisX;
        if (h == axisY) return HandleType.AxisY;
        if (h == axisZ) return HandleType.AxisZ;

        if (h.parent != null)
            return GetHandleType(h.parent);

        return HandleType.None;
    }

    private Vector3 GetWorldPoint(Vector2 screenPos, Vector3 origin)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, new Vector3(0, origin.y, 0)); // 기본 XZ 평면

        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return origin;
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
