// using UnityEngine;
// using UnityEngine.InputSystem;

// public class MarkerSelectionController : MonoBehaviour
// {
//     [Header("Input Actions")]
//     [SerializeField] private InputActionAsset inputActions;
//     [SerializeField] private string actionMapName = "Editor_Pointer";
//     [SerializeField] private string pointActionName = "Point";
//     [SerializeField] private string contactActionName = "Contact";

//     [Header("Selection")]
//     [SerializeField] private float longPressTime = 0.35f;
//     [SerializeField] private float dragThreshold = 15f;
//     [SerializeField] private LayerMask markerLayer;

//     private Camera cam;
//     private InputAction pointAction;
//     private InputAction contactAction;

//     private bool isPointerDown;
//     private float pressTime;
//     private Vector2 startPoint;

//     private MarkerEntity currentSelected;
//     private MarkerEntity pressTarget;

//     private void Awake()
//     {
//         cam = Camera.main;

//         var map = inputActions.FindActionMap(actionMapName, true);
//         pointAction = map.FindAction(pointActionName, true);
//         contactAction = map.FindAction(contactActionName, true);
//     }

//     private void OnEnable()
//     {
//         pointAction.Enable();
//         contactAction.Enable();
//     }

//     private void OnDisable()
//     {
//         pointAction.Disable();
//         contactAction.Disable();
//     }

//     private void Update()
//     {
//         Vector2 point = pointAction.ReadValue<Vector2>();
//         bool pressed = contactAction.IsPressed();

//         if (pressed && !isPointerDown)
//         {
//             OnPointerDown(point);
//         }
//         else if (!pressed && isPointerDown)
//         {
//             OnPointerUp(point);
//         }

//         if (isPointerDown)
//         {
//             OnPointerHold(point);
//         }
//     }

//     private void OnPointerDown(Vector2 point)
//     {
//         isPointerDown = true;
//         pressTime = 0f;
//         startPoint = point;
//         pressTarget = RaycastMarker(point);
//     }

//     private void OnPointerHold(Vector2 point)
//     {
//         pressTime += Time.deltaTime;

//         if (Vector2.Distance(point, startPoint) > dragThreshold)
//         {
//             // 드래그로 판정되면 선택 로직 종료
//             pressTarget = null;
//         }

//         if (pressTarget != null && pressTime >= longPressTime)
//         {
//             SelectMarker(pressTarget);
//             pressTarget = null;
//         }
//     }

//     private void OnPointerUp(Vector2 point)
//     {
//         isPointerDown = false;

//         // 짧은 탭 + 마커 없음 → 선택 해제
//         if (pressTime < longPressTime)
//         {
//             if (RaycastMarker(point) == null)
//                 DeselectCurrent();
//         }
//         pressTarget = null;
//     }

//     private MarkerEntity RaycastMarker(Vector2 screenPos)
//     {
//         Ray ray = cam.ScreenPointToRay(screenPos);

//         if (Physics.Raycast(ray, out RaycastHit hit, 100f, markerLayer))
//         {
//             return hit.collider.GetComponentInParent<MarkerEntity>();
//         }

//         return null;
//     }

//     private void SelectMarker(MarkerEntity marker)
//     {
//         if (currentSelected == marker)
//             return;

//         DeselectCurrent();
//         currentSelected = marker;
//         currentSelected.Select();
//         MarkerRotateAnimator.Instance.StartRotate();
//     }

//     private void DeselectCurrent()
//     {
//         if (currentSelected == null) return;

//         currentSelected.Deselect();
//         MarkerRotateAnimator.Instance.StopRotate();
//         currentSelected = null;
//     }

//     public MarkerEntity GetSelectedMarker()
//     {
//         return currentSelected;
//     }
// }
