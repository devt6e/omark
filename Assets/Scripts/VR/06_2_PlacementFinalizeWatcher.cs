// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlacementFinalizeWatcher : MonoBehaviour
// {
//     [Header("Refs")]
//     [SerializeField] private MarkerPlacementResolver resolver;

//     [Header("Input")]
//     [Tooltip("입력 해제를 감지할 최소 프레임 안정화(튐 방지)")]
//     [SerializeField] private int releaseConfirmFrames = 1;

//     private bool wasDragging;
//     private bool finalizeRequested;
//     private int releaseFrames;

//     private void Awake()
//     {
//         if (resolver == null)
//             resolver = FindFirstObjectByType<MarkerPlacementResolver>();
//     }

//     private void Update()
//     {
//         Debug.Log($"Dragging={MarkerDragContext.IsDragging}, Released={IsInputReleased()}");
//         bool isDragging = MarkerDragContext.IsDragging;

//         // 드래그 시작 감지
//         if (isDragging && !wasDragging)
//         {
//             finalizeRequested = false;
//             releaseFrames = 0;
//         }

//         // 드래그 중일 때만 입력 해제 감시
//         if (isDragging && !finalizeRequested)
//         {
//             if (IsInputReleased())
//             {
//                 releaseFrames++;
//                 if (releaseFrames >= releaseConfirmFrames)
//                 {
//                     RequestFinalize();
//                 }
//             }
//             else
//             {
//                 releaseFrames = 0; // 아직 눌려 있음
//             }
//         }

//         wasDragging = isDragging;
//     }

//     // -----------------------------
//     // 입력 해제 판단 (플랫폼 중립)
//     // -----------------------------
//     private bool IsInputReleased()
//     {
//         // 1) Pointer(터치/펜 포함)
//         if (Pointer.current != null)
//         {
//             // Press가 false면 해제 상태
//             if (!Pointer.current.press.isPressed)
//                 return true;
//         }

//         // 2) Mouse 보조
//         if (Mouse.current != null)
//         {
//             if (!Mouse.current.leftButton.isPressed)
//                 return true;
//         }

//         // 3) Touch 보조 (모든 터치가 종료되었는지)
//         if (Touchscreen.current != null)
//         {
//             bool anyPressed = false;
//             foreach (var t in Touchscreen.current.touches)
//             {
//                 if (t.press.isPressed)
//                 {
//                     anyPressed = true;
//                     break;
//                 }
//             }
//             if (!anyPressed)
//                 return true;
//         }

//         return false;
//     }

//     // -----------------------------
//     // 확정 요청 (단 한 번)
//     // -----------------------------
//     private void RequestFinalize()
//     {
//         if (finalizeRequested) return;
//         finalizeRequested = true;

//         if (resolver == null)
//         {
//             // 안전 정리
//             MarkerDragContext.End();
//             return;
//         }

//         // 현재 포인터 위치로 확정 시도
//         Vector2 screenPos = GetBestScreenPosition();
//         resolver.TryFinalizeAtScreenPos(screenPos);
//     }

//     private Vector2 GetBestScreenPosition()
//     {
//         if (Pointer.current != null)
//             return Pointer.current.position.ReadValue();

//         if (Mouse.current != null)
//             return Mouse.current.position.ReadValue();

//         // 터치가 모두 해제된 경우 마지막 포인터 위치가 의미 없을 수 있으므로
//         // 중앙을 fallback으로 사용
//         return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
//     }
// }
