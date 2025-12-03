using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMoveController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference primaryPointAction;    // Pointer position
    public InputActionReference primaryContactAction;  // Pointer press
    public InputActionReference scrollAction;          // Mouse wheel scroll

    [Header("Camera Settings")]
    public float panSpeed = 0.01f;          // 드래그 이동 속도
    public float pinchZoomSpeed = 0.03f;    // 핀치 줌 속도
    public float mouseWheelZoomSpeed = 1.5f;
    public float minZoom = 3f;
    public float maxZoom = 20f;

    private Camera cam;

    private bool isPanning = false;
    private Vector2 lastPointerPos;
    
    private Vector2 prevTouch0Pos;
    private Vector2 prevTouch1Pos;
    private bool hadTwoTouches = false;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        primaryPointAction.action.Enable();
        primaryContactAction.action.Enable();
        scrollAction.action.Enable();
    }

    private void OnDisable()
    {
        primaryPointAction.action.Disable();
        primaryContactAction.action.Disable();
        scrollAction.action.Disable();
    }

    private void Update()
    {
        // 🔒 Move 모드가 아니면 입력 무시
        if (EditorModeManager.Instance.CurrentMode != EditMode.MoveView)
            return;

        HandlePan();
        HandleMouseWheelZoom();
        HandlePinchZoom();
    }

    // ============================================
    // 1️⃣ 드래그로 카메라 이동 (Pan)
    // ============================================
    private void HandlePan()
    {
        bool pressed = primaryContactAction.action.IsPressed();

        if (pressed)
        {
            Vector2 currentPos = primaryPointAction.action.ReadValue<Vector2>();

            if (!isPanning)
            {
                isPanning = true;
                lastPointerPos = currentPos;
                return;
            }

            Vector2 delta = currentPos - lastPointerPos;
            lastPointerPos = currentPos;

            // delta 값을 월드 이동 값으로 변환
            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * panSpeed;

            // 카메라 이동
            cam.transform.position += move;
        }
        else
        {
            isPanning = false;
        }
    }

    // ============================================
    // 2️⃣ 마우스 휠 줌 (PC 환경)
    // ============================================
    private void HandleMouseWheelZoom()
    {
        Vector2 scroll = scrollAction.action.ReadValue<Vector2>();

        if (scroll.y != 0)
        {
            cam.orthographicSize -= scroll.y * mouseWheelZoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    // ============================================
    // 3️⃣ 핀치 줌 (멀티터치)
    // ============================================
    private void HandlePinchZoom()
    {
        if (Touchscreen.current == null) return;

        var touches = Touchscreen.current.touches;

        // 터치가 2개 이상이어야 한다
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

        Vector2 touch0Pos = t0.position.ReadValue();
        Vector2 touch1Pos = t1.position.ReadValue();

        if (!hadTwoTouches)
        {
            // 첫 프레임은 이전값 초기화만 함
            prevTouch0Pos = touch0Pos;
            prevTouch1Pos = touch1Pos;
            hadTwoTouches = true;
            return;
        }

        // 거리 계산
        float prevDistance = Vector2.Distance(prevTouch0Pos, prevTouch1Pos);
        float currentDistance = Vector2.Distance(touch0Pos, touch1Pos);

        float diff = currentDistance - prevDistance;

        // 줌 처리
        cam.orthographicSize -= diff * pinchZoomSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        // 현재 위치를 다음 프레임을 위한 이전 위치로 저장
        prevTouch0Pos = touch0Pos;
        prevTouch1Pos = touch1Pos;
    }
}
