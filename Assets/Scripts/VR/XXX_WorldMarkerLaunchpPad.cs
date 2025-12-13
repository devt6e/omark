// using UnityEngine;
// using UnityEngine.EventSystems;

// [RequireComponent(typeof(Collider))]
// public class WorldMarkerDragSource : MonoBehaviour,
//     IBeginDragHandler, IDragHandler, IEndDragHandler
// {
//     private MarkerInstance marker;

//     private void Awake()
//     {
//         marker = GetComponent<MarkerInstance>();
//         if (marker == null)
//             marker = GetComponentInParent<MarkerInstance>();
//     }

//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         Debug.Log("move world marker");
//         if (marker == null) return;
//         MarkerDragContext.BeginMove(marker);
//     }

//     public void OnDrag(PointerEventData eventData)
//     {
//         // 고스트는 MarkerGhostController가 처리
//     }

//     public void OnEndDrag(PointerEventData eventData)
//     {
//         Debug.Log("stop world marker");
//         var resolver = Object.FindFirstObjectByType<MarkerPlacementResolver>();
//         if (resolver != null)
//         {
//             resolver.TryFinalizeAtScreenPos(eventData.position);
//         }
//         else
//         {
//             // 실패 시 원복
//             MarkerDragContext.CancelMoveRollback();
//             MarkerDragContext.End();
//         }
//     }
// }
