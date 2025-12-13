// using UnityEngine;

// public class MarkerSelectionManager : MonoBehaviour
// {
//     public static MarkerSelectionManager Instance { get; private set; }

//     [SerializeField] private LayerMask markerLayer;
//     [SerializeField] private float rayDistance = 100f;

//     private MarkerSelectable current;
//     private Camera cam;

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         Instance = this;
//         cam = Camera.main;
//     }

//     public void TrySelectAtScreenPos(Vector2 screenPos)
//     {
//         if (cam == null) cam = Camera.main;
//         if (cam == null) return;
//         Debug.Log("Selectable ok");
//         Ray ray = cam.ScreenPointToRay(screenPos);
//         if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, markerLayer))
//         {
//             var selectable = hit.collider.GetComponentInParent<MarkerSelectable>();
//             Debug.Log("Selectable ok");
//             if (selectable != null)
//                 Select(selectable);
//         }
//         else
//         {
//             ClearSelection();
//         }
//     }

//     private void Select(MarkerSelectable target)
//     {
//         if (current == target) return;

//         ClearSelection();
//         current = target;
//         current.Select();
//     }

//     public void ClearSelection()
//     {
//         if (current != null)
//         {
//             current.Deselect();
//             current = null;
//         }
//     }

//     public MarkerSelectable GetCurrentSelected()
//     {
//         return current;
//     }
// }
