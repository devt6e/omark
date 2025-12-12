using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class T6CameraController : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Camera InputActionAsset")]
    public InputActionAsset inputActions;

    public InputAction pointAction;
    public InputAction contactAction;

    [Header("Camera")]
    private Camera cam;

    [Header("Rotation")]
    public float rotateSensitivity = 0.15f;

    [Header("Move")]
    public float moveDuration = 0.6f;

    [Header("Double Tap")]
    public float doubleTapTime = 0.3f;
    public float tapMaxMoveDistance = 20f;

    [Header("Raycast")]
    public LayerMask moveTargetMask;

    [Header("Effect")]
    public GameObject tapEffectPrefab;

    // Input state
    private Vector2 currentPointPos;
    private Vector2 lastPointPos;
    private bool isContact;

    // Tap detection
    private float lastTapTime;
    private Vector2 lastTapPos;

    // Move
    private bool isMoving;
    private Vector3 moveStartPos;
    private Vector3 moveTargetPos;
    private float moveElapsed;

    private void Awake()
    {
        cam = Camera.main;

        // Action Map / Action 바인딩
        var map = inputActions.FindActionMap("Editor_Pointer", true);
        pointAction = map.FindAction("Point", true);
        contactAction = map.FindAction("Contact", true);
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
        ReadInput();
        HandleRotation();
        HandleMove();
    }

    #region Input

    private void ReadInput()
    {
        if (IsPointerOverUI())
        {
            isContact = false;
            return;
        }

        currentPointPos = pointAction.ReadValue<Vector2>();

        bool pressed = contactAction.IsPressed();

        if (pressed && !isContact)
            OnPress();
        else if (!pressed && isContact)
            OnRelease();

        isContact = pressed;
    }

    private void OnPress()
    {
        // Double Tap 판별
        if (Time.time - lastTapTime <= doubleTapTime &&
            Vector2.Distance(currentPointPos, lastTapPos) <= tapMaxMoveDistance)
        {
            TryMoveToPoint(currentPointPos);
            lastTapTime = 0f;
        }
        else
        {
            lastTapTime = Time.time;
            lastTapPos = currentPointPos;
        }

        lastPointPos = currentPointPos;
    }

    private void OnRelease()
    {
        // nothing
    }

    #endregion

    #region Rotation

    private void HandleRotation()
    {
        if (!isContact || isMoving)
            return;

        Vector2 delta = currentPointPos - lastPointPos;
        lastPointPos = currentPointPos;

        float yaw = delta.x * rotateSensitivity;
        float pitch = -delta.y * rotateSensitivity;

        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Vector3.right, pitch, Space.Self);
    }

    #endregion

    #region Move

    private void TryMoveToPoint(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, moveTargetMask))
            return;

        moveStartPos = transform.position;
        // moveTargetPos = hit.point;
        moveTargetPos = new Vector3(hit.point.x, 1.5f, hit.point.z);
        moveElapsed = 0f;
        isMoving = true;

        if (tapEffectPrefab != null)
            Instantiate(tapEffectPrefab, hit.point, Quaternion.identity);
    }

    private void HandleMove()
    {
        if (!isMoving)
            return;

        moveElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(moveElapsed / moveDuration);

        transform.position = Vector3.Lerp(moveStartPos, moveTargetPos, t);

        if (t >= 1f)
            isMoving = false;
    }

    #endregion

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

    #if UNITY_ANDROID || UNITY_IOS
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return EventSystem.current.IsPointerOverGameObject(
                Touchscreen.current.primaryTouch.touchId.ReadValue()
            );
    #endif

        // Editor / Mouse
        return EventSystem.current.IsPointerOverGameObject();
    }
}
