// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.EventSystems;

// public class MarkerMoveController : MonoBehaviour
// {
//     [Header("Input Actions")]
//     [SerializeField] private InputActionAsset inputActions;
//     [SerializeField] private string actionMapName = "Editor_Pointer";
//     [SerializeField] private string pointActionName = "Point";
//     [SerializeField] private string contactActionName = "Contact";

//     [Header("Move")]
//     [SerializeField] private float dragThreshold = 15f; // px
//     [SerializeField] private LayerMask floorLayer;

//     [Header("Refs")]
//     [SerializeField] private MarkerSelectionController selectionController;
//     [SerializeField] private CameraController3D cameraController;

//     private Camera cam;
//     private InputAction pointAction;
//     private InputAction contactAction;

//     // input state
//     private bool isPointerDown;
//     private Vector2 startPoint;

//     // move state
//     private bool isMoving;        // 실제로 프리뷰 이동 중인지
//     private bool isPlacingNew;    // 발사대에서 생성한 새 마커 배치인지
//     private MarkerEntity currentMarker;
//     private UIMarkerLaunchPad sourceLaunchPad;

//     // revert for existing marker
//     private Vector3 originalPosition;
//     private Quaternion originalRotation;

//     // placement cache
//     private bool hasValidPose;
//     private Vector3 lastValidPos;
//     private Quaternion lastValidRot;

//     private bool startedOverUI;
//     private int pointerId;

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
//             OnPointerDown(point);
//         else if (!pressed && isPointerDown)
//             OnPointerUp();

//         if (isPointerDown)
//         {
//             OnPointerMove(point);
//         }
//     }

//     // ===============================
//     // 외부 진입점 (발사대 -> 새 마커 배치)
//     // ===============================
//     public void BeginPlaceNew(MarkerEntity marker, UIMarkerLaunchPad launchPad)
//     {
//         currentMarker = marker;
//         sourceLaunchPad = launchPad;

//         isPlacingNew = true;
//         isMoving = true;          // 새 마커는 즉시 프리뷰 시작
//         hasValidPose = false;

//         cameraController.IsBlocked = true;
//     }

//     // ===============================
//     // 입력 흐름
//     // ===============================
//     private void OnPointerDown(Vector2 point)
//     {
//         isPointerDown = true;
//         startPoint = point;

//         // 새 마커 배치 중이면 이미 moving 상태로 들어와 있으므로 추가 작업 없음
//         if (isPlacingNew)
//             return;

//         // 기존 마커 이동: 선택된 마커가 있을 때만 "이동 후보"가 됨
//         currentMarker = selectionController.GetSelectedMarker();
//         if (currentMarker == null)
//             return;

//         // threshold 통과 시 되돌릴 원본을 사용할 것이므로 미리 저장
//         originalPosition = currentMarker.transform.position;
//         originalRotation = currentMarker.transform.rotation;

//         isMoving = false;     // 아직은 이동 아님 (threshold 통과 전)
//         hasValidPose = false;
//     }

//     private void OnPointerMove(Vector2 point)
//     {
//         if (currentMarker == null)
//             return;

//         // 기존 마커 이동은 threshold 통과 후에만 이동 시작
//         if (!isPlacingNew && !isMoving)
//         {
//             if (Vector2.Distance(point, startPoint) >= dragThreshold)
//             {
//                 isMoving = true;
//                 cameraController.IsBlocked = true; // 이동 시작 시점부터 카메라 차단
//             }
//             else
//             {
//                 return; // 아직 드래그 아님
//             }
//         }

//         // moving 상태면 프리뷰 갱신
//         if (isMoving)
//         {
//             UpdatePreview(point);
//             pointerId = Pointer.current is Touchscreen ? Touchscreen.current.primaryTouch.touchId.ReadValue() : -1;
//             startedOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId);
//         }
//     }

//     private void OnPointerUp()
//     {
//         isPointerDown = false;

//         // threshold를 못 넘긴 기존 이동은 아무 것도 하지 않음
//         if (!isMoving)
//         {
//             // 기존 이동 후보였던 currentMarker는 유지하지 않게 정리
//             if (!isPlacingNew)
//                 currentMarker = null;
//             return;
//         }
//         EndMove();
//     }

//     // ===============================
//     // 프리뷰 / 확정
//     // ===============================
//     private void UpdatePreview(Vector2 screenPos)
//     {
//         Ray ray = cam.ScreenPointToRay(screenPos);

//         if (Physics.Raycast(ray, out RaycastHit hit, 1000f, floorLayer))
//         {
//             Vector3 pos = hit.point + new Vector3(0f,0.1f,0f); //offset
//             Quaternion rot = Quaternion.identity;

//             currentMarker.transform.SetPositionAndRotation(pos, rot);
//             currentMarker.GetComponent<MarkerVisualController>().SetPreviewValid();

//             lastValidPos = pos;
//             lastValidRot = rot;
//             hasValidPose = true;
//         }
//         else
//         {
//             currentMarker.GetComponent<MarkerVisualController>().SetPreviewInvalid();
//             hasValidPose = false;
//         }
//     }

//     private void EndMove()
//     {        
//         // if(startedOverUI)
//         //     Debug.Log("Is Over UI. pointerID : " + pointerId );

//         if (!hasValidPose)
//         {
//             if (isPlacingNew)
//             {
//                 Destroy(currentMarker.gameObject);
//                 sourceLaunchPad.ClearSpawnedMarker();
//                 InventoryScroll.Instance.SetScroll(true);
//             }
//             else
//             {
//                 currentMarker.transform.SetPositionAndRotation(originalPosition, originalRotation);

//                 // 이동 실패여도 선택 상태는 유지
//                 currentMarker.GetComponent<MarkerVisualController>().SetSelected();
//             }
//         }
//         else if (hasValidPose)
//         {
//             currentMarker.transform.SetPositionAndRotation(lastValidPos, lastValidRot);
//             currentMarker.SyncData();

//             // 배치/이동 종료 후 선택 상태 종료
//             currentMarker.GetComponent<MarkerVisualController>().SetNormal();
//             if (isPlacingNew)
//                 InventoryScroll.Instance.SetScroll(true);
//         }


//         // 공통 정리
//         cameraController.IsBlocked = false;

//         isMoving = false;
//         isPlacingNew = false;
//         currentMarker = null;
//         sourceLaunchPad = null;
//         hasValidPose = false;
//     }
// }
