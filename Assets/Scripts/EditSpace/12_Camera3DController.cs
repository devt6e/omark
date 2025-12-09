using UnityEngine;
using UnityEngine.InputSystem;

public class Camera3DController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference pointAction;        // Pointer position
    public InputActionReference contactAction;      // Pointer press
    public InputActionReference scrollAction;       // Mouse wheel scroll (Vector2, 보통 y 사용)

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.5f;          // 핀치 줌 속도
    public float mouseWheelZoomSpeed = 10f; // 마우스 휠 줌 속도
    public float minDistance = 5f;
    public float maxDistance = 60f;

    [Header("Orbit Settings")]
    public float orbitSpeed = 0.2f;
    public float pitchMin = 10f;
    public float pitchMax = 80f;

    [Header("Pan Settings (UI Buttons)")]
    public float panSpeed = 10f;
    public bool panUp;
    public bool panDown;
    public bool panLeft;
    public bool panRight;

    private Camera cam;
    private Vector3 targetCenter;      // 회전의 중심점
    private float distance;            // 중심점과의 거리
    private bool isDragging = false;
    private Vector2 lastPointerPos;

    private Vector2 prevTouch0Pos;
    private Vector2 prevTouch1Pos;
    private bool hadTwoTouches = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        pointAction.action.Enable();
        contactAction.action.Enable();
        if (scrollAction != null) scrollAction.action.Enable();

        UpdateTargetCenter();
        CalculateInitialDistance();
    }

    private void OnDisable()
    {
        pointAction.action.Disable();
        contactAction.action.Disable();
        if (scrollAction != null) scrollAction.action.Disable();
    }

    private void LateUpdate()
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.MoveView)
            return;

        UpdateTargetCenter();

        HandleOrbit();
        HandlePinchZoom();
        HandleMouseWheelZoom();   // 🔹 에디터/PC용 줌
        HandlePanButtons();

        UpdateCameraPosition();
    }

    // ================================================
    // 🔵 중심점 업데이트
    // ================================================
    private void UpdateTargetCenter()
    {
        Bounds room = RoomManager.Instance.GetRoomBounds();
        if (room.size == Vector3.zero)
            targetCenter = Vector3.zero;
        else
            targetCenter = room.center;
    }

    private void CalculateInitialDistance()
    {
        distance = Vector3.Distance(transform.position, targetCenter);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    // ================================================
    // 🔵 Orbit Rotation (드래그)
    // ================================================
    private void HandleOrbit()
    {
        bool pressed = contactAction.action.IsPressed();
        Vector2 pointer = pointAction.action.ReadValue<Vector2>();

        if (pressed)
        {
            if (!isDragging)
            {
                isDragging = true;
                lastPointerPos = pointer;
                return;
            }

            Vector2 delta = pointer - lastPointerPos;
            lastPointerPos = pointer;

            float yaw = delta.x * orbitSpeed;
            float pitch = -delta.y * orbitSpeed;

            // 수평 회전 (Y축)
            transform.RotateAround(targetCenter, Vector3.up, yaw);

            // 수직 회전 (카메라의 오른쪽 축 기준)
            Vector3 right = transform.right;
            transform.RotateAround(targetCenter, right, pitch);

            // pitch 제한
            Vector3 dir = (transform.position - targetCenter).normalized;
            float currentPitch = Vector3.Angle(Vector3.ProjectOnPlane(dir, Vector3.up), dir);

            if (currentPitch < pitchMin || currentPitch > pitchMax)
            {
                // 범위 넘었으면 되돌리기
                transform.RotateAround(targetCenter, right, -pitch);
            }
        }
        else
        {
            isDragging = false;
        }
    }

    // ================================================
    // 🔵 Pinch Zoom (모바일)
    // ================================================
    private void HandlePinchZoom()
    {
        if (Touchscreen.current == null) return;
        var touches = Touchscreen.current.touches;

        if (touches.Count < 2)
        {
            hadTwoTouches = false;
            return;
        }

        var t0 = touches[0];
        var t1 = touches[1];

        if (!t0.isInProgress || !t1.isInProgress)
        {
            hadTwoTouches = false;
            return;
        }

        Vector2 pos0 = t0.position.ReadValue();
        Vector2 pos1 = t1.position.ReadValue();

        if (!hadTwoTouches)
        {
            prevTouch0Pos = pos0;
            prevTouch1Pos = pos1;
            hadTwoTouches = true;
            return;
        }

        float prevDist = Vector2.Distance(prevTouch0Pos, prevTouch1Pos);
        float currDist = Vector2.Distance(pos0, pos1);
        float diff = currDist - prevDist;

        distance -= diff * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        prevTouch0Pos = pos0;
        prevTouch1Pos = pos1;
    }

    // ================================================
    // 🔵 Mouse Wheel Zoom (에디터/PC)
    // ================================================
    private void HandleMouseWheelZoom()
    {
        if (scrollAction == null) return;

        Vector2 scroll = scrollAction.action.ReadValue<Vector2>();
        if (Mathf.Abs(scroll.y) < 0.0001f) return;

        // y > 0 이면 앞으로 당기기(줌인), y < 0 이면 줌아웃
        distance -= scroll.y * mouseWheelZoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    // ================================================
    // 🔵 Pan (UI 버튼)
    // ================================================
    private void HandlePanButtons()
    {
        Vector3 move = Vector3.zero;

        if (panUp)
            move += transform.forward;
        if (panDown)
            move -= transform.forward;
        if (panLeft)
            move -= transform.right;
        if (panRight)
            move += transform.right;

        move.y = 0;

        if (move != Vector3.zero)
        {
            targetCenter += move.normalized * panSpeed * Time.deltaTime;
        }
    }

    // ================================================
    // 🔵 카메라 최종 위치 적용
    // ================================================
    private void UpdateCameraPosition()
    {
        Vector3 dir = (transform.position - targetCenter).normalized;
        transform.position = targetCenter + dir * distance;
        transform.LookAt(targetCenter);
    }

    // ================================================
    // 🔵 UI 버튼 OnPointerDown / OnPointerUp 연결용
    // ================================================
    public void SetPanUp(bool active) => panUp = active;
    public void SetPanDown(bool active) => panDown = active;
    public void SetPanLeft(bool active) => panLeft = active;
    public void SetPanRight(bool active) => panRight = active;
}
