using UnityEngine;
using UnityEngine.InputSystem;

public class Camera3DController : MonoBehaviour
{
    public static Camera3DController Instance {get; private set;}

    [Header("Input Actions")]
    public InputActionReference pointAction;
    public InputActionReference contactAction;
    public InputActionReference scrollAction;

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.5f;
    public float mouseWheelZoomSpeed = 10f;
    public float minDistance = 5f;
    public float maxDistance = 60f;

    [Header("Orbit Settings")]
    public float orbitSpeed = 0.2f;
    public float pitchMin = 10f;
    public float pitchMax = 80f;

    [Header("Pan Settings")]
    public float panSpeed = 0.002f;        
    public float panSpeedButtons = 10f;    

    public bool panUp, panDown, panLeft, panRight;

    private Camera cam;

    private Vector3 targetCenter;
    private float distance;

    private bool isDragging = false;
    private Vector2 lastPointerPos;

    private bool isTwoFinger = false;
    private Vector2 prevCenterPos;
    private float prevDist;

    private void Awake()
    {
        Instance = this;
        EnableInput();
    }

    private void OnEnable()
    {
        SetInitialCenter();
        CalculateInitialDistance();
    }

    private void LateUpdate()
    {
        if (GizmoInputBlocker.IsDraggingGizmo)
            return;
        if (EditorModeManager.Instance.CurrentMode != EditMode.MoveView3D)
            return;

        if (cam == null) return; 

        DetectTwoFingerState();

        if (!isTwoFinger)
            HandleOrbit();

        HandlePan();
        HandleZoom();
        HandlePanButtons();

        UpdateCameraPosition();
    }

    public void SetCamera(Camera newCam) => cam = newCam;

    // ================================================================
    // 초기 1회만 center 설정
    // ================================================================
    private void SetInitialCenter()
    {
        Bounds room = RoomManager.Instance.GetRoomBounds();

        if (room.size == Vector3.zero)
            targetCenter = Vector3.zero;
        else
            targetCenter = room.center + new Vector3(0f, 1.5f, 0f);
    }

    private void CalculateInitialDistance()
    {
        distance = Vector3.Distance(transform.position, targetCenter);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    // ================================================================
    // 멀티터치 or 에디터 오른쪽 버튼 → 두 손 모드
    // ================================================================
    private void DetectTwoFingerState()
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            isTwoFinger = true;
            return;
        }

        if (Touchscreen.current == null)
        {
            isTwoFinger = false;
            return;
        }

        int active = 0;
        foreach (var t in Touchscreen.current.touches)
            if (t.isInProgress) active++;

        isTwoFinger = (active >= 2);
    }

    // ================================================================
    // Orbit (한 손 전용)
    // ================================================================
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

            transform.RotateAround(targetCenter, Vector3.up, yaw);

            Vector3 right = transform.right;
            transform.RotateAround(targetCenter, right, pitch);

            Vector3 dir = (transform.position - targetCenter).normalized;
            float currentPitch = Vector3.Angle(Vector3.ProjectOnPlane(dir, Vector3.up), dir);

            if (currentPitch < pitchMin || currentPitch > pitchMax)
                transform.RotateAround(targetCenter, right, -pitch);
        }
        else
        {
            isDragging = false;
        }
    }

    // ================================================================
    // Pan (두 손 / 오른쪽 클릭)
    // ================================================================
    private void HandlePan()
    {
        // 에디터: 오른쪽 드래그 Pan
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            ApplyPan(delta);
            return;
        }

        // 모바일 두 손 Pan
        if (!isTwoFinger || Touchscreen.current == null)
            return;

        var touches = Touchscreen.current.touches;
        if (touches.Count < 2) return;

        var t0 = touches[0];
        var t1 = touches[1];

        if (!t0.isInProgress || !t1.isInProgress)
            return;

        Vector2 p0 = t0.position.ReadValue();
        Vector2 p1 = t1.position.ReadValue();
        Vector2 center = (p0 + p1) * 0.5f;

        if (prevCenterPos == Vector2.zero)
        {
            prevCenterPos = center;
            prevDist = Vector2.Distance(p0, p1);
            return;
        }

        Vector2 delta2D = center - prevCenterPos;
        prevCenterPos = center;

        ApplyPan(delta2D);
    }

    // 화면 기준 Pan을 실제 월드 이동으로 변환
    private void ApplyPan(Vector2 delta)
    {
        Vector3 camRight = cam.transform.right;
        Vector3 camUp = Vector3.ProjectOnPlane(cam.transform.up, Vector3.up).normalized;

        Vector3 worldMove =
            camRight * (-delta.x * panSpeed) +
            camUp    * (-delta.y * panSpeed);

        transform.position += worldMove;
        targetCenter += worldMove;
    }

    // ================================================================
    // Zoom (두 손 거리변화 + 마우스휠)
    // ================================================================
    private void HandleZoom()
    {
        // 모바일 두 손 Zoom
        if (isTwoFinger && Touchscreen.current != null)
        {
            var touches = Touchscreen.current.touches;
            if (touches.Count >= 2)
            {
                var t0 = touches[0];
                var t1 = touches[1];

                Vector2 p0 = t0.position.ReadValue();
                Vector2 p1 = t1.position.ReadValue();

                float currDist = Vector2.Distance(p0, p1);

                if (prevDist != 0)
                {
                    float diff = currDist - prevDist;
                    distance -= diff * zoomSpeed * Time.deltaTime;
                    distance = Mathf.Clamp(distance, minDistance, maxDistance);
                }

                prevDist = currDist;
            }
        }

        // 마우스 휠 Zoom
        Vector2 scroll = scrollAction.action.ReadValue<Vector2>();
        if (Mathf.Abs(scroll.y) > 0.0001f)
        {
            distance -= scroll.y * mouseWheelZoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    // ================================================================
    // UI 버튼 Pan
    // ================================================================
    private void HandlePanButtons()
    {
        Vector3 move = Vector3.zero;

        if (panUp) move += cam.transform.forward;
        if (panDown) move -= cam.transform.forward;
        if (panLeft) move -= cam.transform.right;
        if (panRight) move += cam.transform.right;

        move.y = 0;

        if (move != Vector3.zero)
        {
            move *= (panSpeedButtons * Time.deltaTime);
            transform.position += move;
            targetCenter += move;
        }
    }

    // ================================================================
    // 카메라 최종 위치 적용
    // ================================================================
    private void UpdateCameraPosition()
    {
        Vector3 dir = (transform.position - targetCenter).normalized;
        transform.position = targetCenter + dir * distance;
        transform.LookAt(targetCenter);
    }

    public void EnableInput()
    {
        pointAction.action.Enable();
        contactAction.action.Enable();
        scrollAction?.action.Enable();
    }

    public void DisableInput()
    {
        pointAction.action.Disable();
        contactAction.action.Disable();
        scrollAction?.action.Disable();
    }
}
