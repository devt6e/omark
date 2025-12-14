using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Marker 이동 / 배치 전용 컨트롤러
/// - 입력 판단 주체
/// - preview 동안 데이터 불변
/// - 확정 시 Repository에 결과 기록
/// </summary>
public class MarkerMoveController : MonoBehaviour
{
    // =========================
    // Input Actions
    // =========================
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Editor_Pointer";
    [SerializeField] private string pointActionName = "Point";
    [SerializeField] private string contactActionName = "Contact";

    // =========================
    // Move Settings
    // =========================
    [Header("Move")]
    [SerializeField] private float dragThreshold = 15f;   // px
    [SerializeField] private float startMoveDistance = 120f; // px
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private float yOffset = 0.1f;

    // =========================
    // Refs
    // =========================
    [Header("Refs")]
    [SerializeField] private MarkerSelectionController selectionController;
    [SerializeField] private CameraController3D cameraController;
    [SerializeField] private MarkerSlotSpawner slotSpawner;

    // =========================
    // Internal
    // =========================
    private Camera cam;
    private InputAction pointAction;
    private InputAction contactAction;

    // input state
    private bool isPointerDown;
    private Vector2 startPoint;

    // move state
    private bool isMoving;
    private bool isPlacingNew;
    private bool canStartMove;
    private MarkerInstance currentMarker;

    // revert (existing marker)
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // preview cache
    private bool hasValidPose;
    private Vector3 lastValidPos;
    private Quaternion lastValidRot;

    // ui
    private int pointerId;
    private bool startedOverUI;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        cam = Camera.main;

        var map = inputActions.FindActionMap(actionMapName, true);
        pointAction = map.FindAction(pointActionName, true);
        contactAction = map.FindAction(contactActionName, true);
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
        Vector2 point = pointAction.ReadValue<Vector2>();
        bool pressed = contactAction.IsPressed();

        if (pressed && !isPointerDown)
            OnPointerDown(point);
        else if (!pressed && isPointerDown)
            OnPointerUp();

        if (isPointerDown)
            OnPointerMove(point);
    }

    // =========================
    // External Entry (New Marker)
    // =========================
    public void BeginPlaceNew(MarkerInstance marker)
    {
        currentMarker = marker;

        isPlacingNew = true;
        isMoving = true;
        hasValidPose = false;
        canStartMove = true; // 새 배치는 무조건 이동 허용

        cameraController.IsBlocked = true;
    }

    // =========================
    // Input Flow
    // =========================
    private void OnPointerDown(Vector2 point)
    {
        isPointerDown = true;
        startPoint = point;
        canStartMove = false;

        if (isPlacingNew)
            return;

        currentMarker = selectionController.GetSelected();
        if (currentMarker == null)
            return;

        originalPosition = currentMarker.transform.position;
        originalRotation = currentMarker.transform.rotation;

        // ⭐ 이동 시작 후보 판정
        Vector2 markerScreenPos =
            cam.WorldToScreenPoint(currentMarker.transform.position);

        float dist = Vector2.Distance(startPoint, markerScreenPos);
        if (dist <= startMoveDistance)
            canStartMove = true;

        isMoving = false;
        hasValidPose = false;
    }

    private void OnPointerMove(Vector2 point)
    {
        if (currentMarker == null)
            return;

        if (!isPlacingNew && !isMoving)
        {
            if (Vector2.Distance(point, startPoint) >= dragThreshold)
            {
                // ⭐ 이동 시작 조건 강화
                if (!canStartMove)
                {
                    currentMarker = null;
                    return;
                }

                isMoving = true;
                cameraController.IsBlocked = true;
            }
            else
            {
                return;
            }
        }

        UpdatePreview(point);

        pointerId = Pointer.current is Touchscreen
            ? Touchscreen.current.primaryTouch.touchId.ReadValue()
            : -1;
        startedOverUI = EventSystem.current != null &&
                        EventSystem.current.IsPointerOverGameObject(pointerId);
    }

    private void OnPointerUp()
    {
        isPointerDown = false;

        if (!isMoving)
        {
            if (!isPlacingNew)
                currentMarker = null;
            return;
        }

        EndMove();
    }

    // =========================
    // Preview / Commit
    // =========================
    private void UpdatePreview(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, floorLayer))
        {
            Vector3 pos = hit.point + Vector3.up * yOffset;
            Quaternion rot = Quaternion.identity;

            currentMarker.SetPreviewPose(pos, rot);
            currentMarker.SetPreviewValid();

            lastValidPos = pos;
            lastValidRot = rot;
            hasValidPose = true;
        }
        else
        {
            currentMarker.SetPreviewInvalid();
            hasValidPose = false;
        }
    }

    private void EndMove()
    {
        string defId = currentMarker.DefinitionId;

        if (!hasValidPose)
        {
            if (isPlacingNew)
            {
                currentMarker.ClearPlacement();
                Destroy(currentMarker.gameObject);
                slotSpawner.UnlockDefinition(defId);
            }
            else
            {
                currentMarker.transform.SetPositionAndRotation(
                    originalPosition,
                    originalRotation
                );
                currentMarker.Select();
            }
        }
        else
        {
            currentMarker.transform.SetPositionAndRotation(
                lastValidPos,
                lastValidRot
            );

            currentMarker.CommitPlacement();
            currentMarker.Deselect();
        }

        cameraController.IsBlocked = false;

        isMoving = false;
        isPlacingNew = false;
        canStartMove = false;
        currentMarker = null;
        hasValidPose = false;
    }
}
