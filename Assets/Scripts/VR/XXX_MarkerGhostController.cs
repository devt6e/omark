// using UnityEngine;
// using TMPro;
// using UnityEngine.InputSystem;

// public class MarkerGhostController : MonoBehaviour
// {
//     [Header("Ghost")]
//     [SerializeField] private GameObject ghostPrefab; // 고스트 기본 프리팹(필수)
//     [SerializeField] private LayerMask placementLayer;
//     [SerializeField] private float rayDistance = 100f;

//     private GameObject ghost;
//     private Renderer ghostRenderer;
//     private TextMeshPro ghostText;

//     private Camera cam;
//     private int lastVersion = -1;

//     private void Awake()
//     {
//         cam = Camera.main;
//     }

//     private void Update()
//     {
//         if (!MarkerDragContext.IsDragging)
//         {
//             DestroyGhost();
//             return;
//         }

//         EnsureGhost();

//         // 드래그 대상이 바뀌었으면(새 마커/이동 마커 변경) 고스트 시각 갱신
//         if (MarkerDragContext.Version != lastVersion)
//         {
//             lastVersion = MarkerDragContext.Version;
//             SyncVisuals(MarkerDragContext.GetActiveData());
//         }

//         UpdateGhostTransform();
//     }

//     private void EnsureGhost()
//     {
//         if (ghost != null) return;

//         ghost = Instantiate(ghostPrefab);
//         ghost.name = "Ghost_Marker";
//         ghostRenderer = ghost.GetComponentInChildren<Renderer>();
//         ghostText = ghost.GetComponentInChildren<TextMeshPro>();

//         // 기본은 숨김 상태에서 시작해도 OK
//         ghost.SetActive(false);
//     }

//     private void UpdateGhostTransform()
//     {
//         if (cam == null)
//         {
//             ghost.SetActive(false);
//             return;
//         }

//         // Pointer 우선(터치 포함), 없으면 Mouse
//         Vector2 screenPos = Vector2.zero;
//         if (Pointer.current != null) screenPos = Pointer.current.position.ReadValue();
//         else if (Mouse.current != null) screenPos = Mouse.current.position.ReadValue();
//         else
//         {
//             ghost.SetActive(false);
//             return;
//         }

//         Ray ray = cam.ScreenPointToRay(screenPos);

//         if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, placementLayer))
//         {
//             Vector3 finalPos = hit.point;

//             // Y 보정
//             if (ghostRenderer != null)
//                 finalPos.y += ghostRenderer.bounds.extents.y;

//             ghost.transform.position = finalPos;
//             ghost.transform.rotation = LookAtCamera(finalPos);

//             if (!ghost.activeSelf) ghost.SetActive(true);
//         }
//         else
//         {
//             if (ghost.activeSelf) ghost.SetActive(false);
//         }
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

//     private void SyncVisuals(T6MarkerData data)
//     {
//         if (data == null) return;

//         if (ghostRenderer != null)
//         {
//             Color c = data.color;
//             c.a = 0.3f; // 고스트 반투명
//             ghostRenderer.material.color = c;
//         }

//         if (ghostText != null)
//         {
//             ghostText.text = data.displayName;
//         }
//     }

//     private void DestroyGhost()
//     {
//         if (ghost == null) return;
//         Destroy(ghost);
//         ghost = null;
//         ghostRenderer = null;
//         ghostText = null;
//         lastVersion = -1;
//     }
// }
