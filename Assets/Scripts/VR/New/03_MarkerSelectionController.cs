using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MarkerInstance 선택 전용 컨트롤러.
/// - 입력 판단만 담당
/// - 데이터 접근/변경 없음
/// - 선택 상태는 "표현" 문제로만 처리
/// </summary>
public class MarkerSelectionController : MonoBehaviour
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
    // Selection Settings
    // =========================
    [Header("Selection")]
    [SerializeField] private float longPressTime = 0.35f;
    [SerializeField] private float dragThreshold = 15f;
    [SerializeField] private LayerMask markerLayer;

    [Header("Deselect by Distance")]
    [SerializeField] private float deselectDistance = 120f; // screen px

    // =========================
    // Internal
    // =========================
    private Camera cam;
    private InputAction pointAction;
    private InputAction contactAction;

    private bool isPointerDown;
    private float pressTime;
    private Vector2 startPoint;
    private Vector2 selectedMarkerScreenPos;

    private MarkerInstance currentSelected;
    private MarkerInstance pressTarget;

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
            OnPointerUp(point);

        if (isPointerDown)
            OnPointerHold(point);
    }

    // =========================
    // Input Flow
    // =========================
    private void OnPointerDown(Vector2 point)
    {
        // Debug.Log("[Selection Cont.OnPointerDown] : Down");
        isPointerDown = true;
        pressTime = 0f;
        startPoint = point;
        pressTarget = RaycastMarker(point);

        if(pressTarget != currentSelected)
            Deselect();

        if (currentSelected != null)
        {
            Vector3 screenPos =
                cam.WorldToScreenPoint(currentSelected.transform.position);

            selectedMarkerScreenPos = screenPos;
        }
        
    }

    private void OnPointerHold(Vector2 point)
    {
        // Debug.Log("[Selection Cont.OnPointerHold] : Hold");
        pressTime += Time.deltaTime;

        // 드래그 판정 → 선택 취소
        if (Vector2.Distance(point, startPoint) > dragThreshold)
        {
             if (currentSelected != null)
            {
                float distFromSelected =
                    Vector2.Distance(startPoint, selectedMarkerScreenPos);

                if (distFromSelected > deselectDistance)
                {
                    // 이 입력은 "마커 이동 의도"가 아님
                    Deselect();
                    pressTarget = null;
                    return;
                }
            }
            pressTarget = null;
            return;
        }

        // 롱프레스 → 선택
        if (pressTarget != null && pressTime >= longPressTime)
        {
            Select(pressTarget);
            pressTarget = null;
        }
    }

    private void OnPointerUp(Vector2 point)
    {
        // Debug.Log("[Selection Cont.OnPointerUp] : Up");
        isPointerDown = false;

        // 짧은 탭 + 마커 미적중 → 선택 해제
        if (pressTime < longPressTime)
        {
            if (RaycastMarker(point) == null)
                Deselect();
        }

        pressTarget = null;
    }

    // =========================
    // Selection Logic
    // =========================
    private void Select(MarkerInstance marker)
    {
        if (currentSelected == marker)
            return;

        Debug.Log($"[Selection Cont.Select] seleted");
        Deselect();

        Handheld.Vibrate();
        currentSelected = marker;
        currentSelected.Select();
        MarkerRotateAnimator.Instance.SetSingleTarget(marker);
    }

    public void Deselect()
    {
        if (currentSelected == null)
            return;

        currentSelected.Deselect();
        MarkerRotateAnimator.Instance.StopRotate();
        currentSelected = null;
    }

    // =========================
    // Raycast
    // =========================
    private MarkerInstance RaycastMarker(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, markerLayer))
        {
            return hit.collider.GetComponentInParent<MarkerInstance>();
        }

        return null;
    }

    // =========================
    // External Query
    // =========================
    public MarkerInstance GetSelected()
    {
        return currentSelected;
    }

    public bool HasSelection => currentSelected != null;
}
