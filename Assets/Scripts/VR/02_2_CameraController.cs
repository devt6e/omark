using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController3D : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Editor_Pointer";
    [SerializeField] private string pointActionName = "Point";
    [SerializeField] private string contactActionName = "Contact";

    [Header("Rotation")]
    [SerializeField] private float rotateSensitivity = 0.2f;

    [Header("Pitch Clamp (Up / Down)")]
    [SerializeField] private float minPitch = -70f;
    [SerializeField] private float maxPitch = -30f;

    [Header("Drag Threshold")]
    [SerializeField] private float dragThreshold = 15f; // px

    [Header("Double Tap Move")]
    [SerializeField] private float doubleTapTime = 0.3f;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private float moveLerpTime = 0.25f;

    // internal
    private Camera cam;

    private InputAction pointAction;
    private InputAction contactAction;

    private bool isPointerDown;
    private bool isDragging;
    private Vector2 startPoint;
    private Vector2 lastPoint;

    private float lastTapTime;
    private Vector2 lastTapPos;

    private int pointerId;
    private bool startedOverUI;

    private Coroutine moveRoutine;

    // 회전 누적값
    private float currentYaw;
    private float currentPitch;

    // 외부 Gate (마커 이동 중 카메라 차단용)
    public bool IsBlocked { get; set; }

    private void Awake()
    {
        cam = Camera.main;

        var map = inputActions.FindActionMap(actionMapName, true);
        pointAction = map.FindAction(pointActionName, true);
        contactAction = map.FindAction(contactActionName, true);

        // 초기 회전값을 현재 Transform 기준으로 세팅
        Vector3 euler = transform.eulerAngles;
        currentYaw = euler.y;
        currentPitch = NormalizePitch(euler.x);
    }

    private void OnEnable()
    {
        pointAction.Enable();
        contactAction.Enable();
    }

    private void OnDisable()
    {
        pointAction.Disable();
        contactAction.Disable();
    }

    private void Update()
    {
        if (IsBlocked)
            return;

        Vector2 point = pointAction.ReadValue<Vector2>();
        bool pressed = contactAction.IsPressed();

        if (pressed && !isPointerDown)
        {
            OnPointerDown(point);
        }
        else if (!pressed && isPointerDown)
        {
            OnPointerUp(point);
        }

        if (isPointerDown)
        {
            OnPointerMove(point);
        }
    }

    private void OnPointerDown(Vector2 point)
    {
        isPointerDown = true;
        isDragging = false;
        startPoint = point;
        lastPoint = point;
    }

    private void OnPointerMove(Vector2 point)
    {
        Vector2 deltaFromStart = point - startPoint;

        // 드래그 판정
        if (!isDragging && deltaFromStart.magnitude >= dragThreshold)
        {
            isDragging = true;
        }

        // 카메라 회전
        if (isDragging)
        {
            Vector2 delta = point - lastPoint;
            RotateCamera(delta);
        }

        lastPoint = point;
    }

    private void OnPointerUp(Vector2 point)
    {
        isPointerDown = false;

        // 드래그가 아니면 탭 후보
        if (!isDragging)
        {
            TryDoubleTap(point);
        }

        isDragging = false;
    }

    private void RotateCamera(Vector2 delta)
    {
        currentYaw += delta.x * rotateSensitivity;
        currentPitch += -delta.y * rotateSensitivity;

        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(
            currentPitch,
            currentYaw,
            0f
        );
    }

    private void TryDoubleTap(Vector2 point)
    {
        float time = Time.time;

        if (time - lastTapTime <= doubleTapTime &&
            Vector2.Distance(point, lastTapPos) <= dragThreshold)
        {
            MoveToFloor(point);
            lastTapTime = 0f;
        }
        else
        {
            lastTapTime = time;
            lastTapPos = point;
        }
    }

    private void MoveToFloor(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, floorLayer))
        {
            Vector3 targetPos = new Vector3(hit.point.x, 1.3f, hit.point.z);

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(MoveRoutine(targetPos));
        }
    }

    private System.Collections.IEnumerator MoveRoutine(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / moveLerpTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        moveRoutine = null;
    }

    // 0~360 → -180~180 변환
    private float NormalizePitch(float pitch)
    {
        if (pitch > 180f)
            pitch -= 360f;
        return pitch;
    }
}
