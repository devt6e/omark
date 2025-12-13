// using UnityEngine;

// public class MarkerMoveController : MonoBehaviour
// {
//     [SerializeField] private LayerMask placementLayer;
//     [SerializeField] private float rayDistance = 100f;
//     [SerializeField] private MarkerGhostController ghost;

//     private Camera cam;
//     private MarkerSelectable current;
//     private bool isMoving;

//     private void Awake()
//     {
//         cam = Camera.main;
//     }

//     public void BeginMove(Vector2 screenPos)
//     {
//         if (isMoving) return;

//         current = MarkerSelectionManager.Instance?.GetCurrentSelected();
//         if (current == null) return;

//         isMoving = true;
//         CameraInputGate.Lock();

//         ghost.BeginGhostForMove(current.gameObject);
//         UpdateMove(screenPos);
//     }

//     public void UpdateMove(Vector2 screenPos)
//     {
//         if (!isMoving) return;

//         if (Resolve(screenPos, out Vector3 pos, out Quaternion rot))
//             ghost.SetGhostTransform(pos, rot);
//     }

//     public void EndMove(Vector2 screenPos)
//     {
//         if (!isMoving) return;

//         if (Resolve(screenPos, out Vector3 pos, out Quaternion rot))
//             current.transform.SetPositionAndRotation(pos, rot);

//         ghost.EndGhost();
//         isMoving = false;
//         CameraInputGate.Unlock();
//     }

//     private bool Resolve(Vector2 screenPos, out Vector3 pos, out Quaternion rot)
//     {
//         pos = Vector3.zero;
//         rot = Quaternion.identity;

//         Ray ray = cam.ScreenPointToRay(screenPos);
//         if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, placementLayer))
//             return false;

//         pos = hit.point;

//         Renderer r = current.GetComponentInChildren<Renderer>();
//         if (r != null)
//             pos.y += r.bounds.extents.y;

//         Vector3 dir = cam.transform.position - pos;
//         dir.y = 0;
//         if (dir != Vector3.zero)
//             rot = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180, 0);

//         return true;
//     }
// }
