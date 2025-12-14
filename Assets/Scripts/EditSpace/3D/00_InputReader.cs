using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }

    [Header("Current Input States")]
    public Vector2 Point;             // pointer position
    public bool ContactStarted;       // pressed this frame
    public bool ContactActive;        // being pressed
    public bool ContactEnded;         // released this frame
    // public float Scroll;              // mouse wheel / pinch delta

    private InputSystem_Actions actions;

    private float lastPressTime;
    public float HoldTime { get; private set; } // long press 판정용

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        actions = new InputSystem_Actions();
        actions.Enable();

        // InputAction 연결
        actions.Editor_Pointer.Point.performed += ctx => Point = ctx.ReadValue<Vector2>();
        actions.Editor_Pointer.Contact.started += ctx => OnContactStart();
        actions.Editor_Pointer.Contact.canceled += ctx => OnContactEnd();
        // actions.Editor_Pointer.Scroll.performed += ctx => Scroll = ctx.ReadValue<float>();
    }

    private void Update()
    {
        // Hold 계산
        if (ContactActive)
        {
            HoldTime = Time.time - lastPressTime;
        }
        else
        {
            HoldTime = 0f;
        }

        // Scroll은 1프레임 후 자동 초기화하면 편함
        // Scroll = 0f;

        ContactStarted = false;
        ContactEnded = false;
    }

    private void OnContactStart()
    {
        ContactStarted = true;
        ContactActive = true;
        lastPressTime = Time.time;
    }

    private void OnContactEnd()
    {
        ContactEnded = true;
        ContactActive = false;
    }
}
