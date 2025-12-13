// using UnityEngine;
// using UnityEngine.InputSystem;

// public class MarkerPlacementResolver : MonoBehaviour
// {
//     [SerializeField] private LayerMask placementLayer;
//     [SerializeField] private float rayDistance = 100f;

//     private Camera cam;

//     private void Awake()
//     {
//         cam = Camera.main;
//     }

//     public void TryFinalizeAtScreenPos(Vector2 screenPos)
//     {
        
//         if (!MarkerDragContext.IsDragging)
//             return;
//         // Debug.Log("in Dragging");
//         if (cam == null)
//         {
//             FailAndCleanup();
//             return;
//         }
//         // Debug.Log("exist Camera");
//         Ray ray = cam.ScreenPointToRay(screenPos);
//         bool hitOk = Physics.Raycast(ray, out RaycastHit hit, rayDistance, placementLayer);

//         if (!hitOk)
//         {
//             FailAndCleanup();
//             return;
//         }
//         // Debug.Log("Lay okay");
//         // hit 성공 → Kind에 따라 확정
//         if (MarkerDragContext.Kind == MarkerDragKind.New)
//         {
//             FinalizeNew(hit);
//             MarkerDragContext.End();
//             return;
//         }
//         // Debug.Log("Hit kind is not new");
//         if (MarkerDragContext.Kind == MarkerDragKind.Move)
//         {
//             FinalizeMove(hit);
//             MarkerDragContext.End();
//             return;
//         }
//         // Debug.Log("Hit kind is not move");
//         // 안전
//         MarkerDragContext.End();
//     }

//     private void FinalizeNew(RaycastHit hit)
//     {
//         T6MarkerData data = MarkerDragContext.NewData;
//         if (data == null || data.markerPrefab == null) return;

//         GameObject obj = Instantiate(data.markerPrefab);

//         // MarkerInstance 확보/부착
//         MarkerInstance inst = obj.GetComponent<MarkerInstance>();
//         if (inst == null) inst = obj.AddComponent<MarkerInstance>();
//         inst.Initialize(data);

//         // 위치/회전
//         Vector3 finalPos = hit.point;
//         Renderer r = obj.GetComponentInChildren<Renderer>();
//         if (r != null) finalPos.y += r.bounds.extents.y;

//         obj.transform.position = finalPos;
//         obj.transform.rotation = LookAtCamera(finalPos);

//         // 3D 드래그 재배치 가능하도록 DragSource 자동 부착(권장)
//         if (obj.GetComponent<WorldMarkerDragSource>() == null)
//             obj.AddComponent<WorldMarkerDragSource>();

//         // Collider 필수(없으면 이벤트 못 받음)
//         if (obj.GetComponent<Collider>() == null)
//         {
//             // 가장 간단한 기본 콜라이더(프리팹에 콜라이더 넣는 게 최선)
//             var bc = obj.AddComponent<BoxCollider>();
//             // 자동 크기 추정이 필요하면 MeshRenderer bounds 기반으로 조절 가능(여기선 최소만)
//         }
//     }

//     private void FinalizeMove(RaycastHit hit)
//     {
//         MarkerInstance moving = MarkerDragContext.MovingMarker;
//         if (moving == null) return;

//         // 숨겨둔 실체를 위치 갱신 후 재활성
//         Vector3 finalPos = hit.point;

//         // Y 보정은 “실제 마커 렌더러” 기준
//         Renderer r = moving.GetComponentInChildren<Renderer>();
//         if (r != null) finalPos.y += r.bounds.extents.y;

//         moving.transform.position = finalPos;
//         moving.transform.rotation = LookAtCamera(finalPos);
//         moving.gameObject.SetActive(true);
//         // moving.SetAlpha(1f);
//     }

//     private Quaternion LookAtCamera(Vector3 pos)
//     {
//         Vector3 dir = cam.transform.position - pos;
//         dir.y = 0;
//         if (dir == Vector3.zero) return Quaternion.identity;

//         Quaternion rot = Quaternion.LookRotation(dir);
//         rot *= Quaternion.Euler(0, 180, 0);
//         return rot;
//     }

//     private void FailAndCleanup()
//     {
//         // Move면 원복
//         if (MarkerDragContext.Kind == MarkerDragKind.Move)
//             MarkerDragContext.CancelMoveRollback();

//         MarkerDragContext.End();
//     }
// }
