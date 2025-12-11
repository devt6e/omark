using UnityEngine;

public enum GizmoMode
{
    Move,
    Rotate
}

public class FurnitureGizmoController : MonoBehaviour
{
    public static FurnitureGizmoController Instance { get; private set; }

    [Header("Gizmos")]
    public FurnitureMoveGizmo moveGizmo;
    public FurnitureRotateGizmo rotateGizmo;

    [Header("Settings")]
    [SerializeField] private float longPressTime = 0.6f;
    [SerializeField] private float longPressMoveThreshold = 5f; // px

    public GizmoMode CurrentMode { get; private set; } = GizmoMode.Move;

    private FurniturePiece target;
    private bool toggledThisPress = false;
    private Vector2 pressStartScreenPos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // gameObject.SetActive(false);
    }

    private void Update()
    {
        if (target == null)
            return;

        var input = InputReader.Instance;

        // 롱프레스 기반 모드 토글 (드래그 중이 아닐 때만)
        if (input.ContactStarted)
        {
            pressStartScreenPos = input.Point;
            toggledThisPress = false;
        }

        if (input.ContactActive && !GizmoInputBlocker.IsDraggingGizmo && !toggledThisPress)
        {
            float hold = input.HoldTime;
            float dist = (input.Point - pressStartScreenPos).magnitude;

            if (hold >= longPressTime && dist <= longPressMoveThreshold)
            {
                ToggleMode();
                toggledThisPress = true;
            }
        }

        if (input.ContactEnded)
        {
            toggledThisPress = false;
        }
    }

    public void Attach(FurniturePiece piece)
    {
        target = piece;

        if (target == null)
        {
            moveGizmo.Detach();
            rotateGizmo.Detach();
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        moveGizmo.AttachTarget(target);
        rotateGizmo.AttachTarget(target);

        SetMode(GizmoMode.Move);
    }

    public void Detach()
    {
        target = null;
        moveGizmo.Detach();
        rotateGizmo.Detach();
        gameObject.SetActive(false);
    }

    public void SetMode(GizmoMode mode)
    {
        CurrentMode = mode;

        if (moveGizmo != null)
            moveGizmo.gameObject.SetActive(mode == GizmoMode.Move);

        if (rotateGizmo != null)
            rotateGizmo.gameObject.SetActive(mode == GizmoMode.Rotate);
    }

    public void ToggleMode()
    {
        if (CurrentMode == GizmoMode.Move)
            SetMode(GizmoMode.Rotate);
        else
            SetMode(GizmoMode.Move);
    }
}
