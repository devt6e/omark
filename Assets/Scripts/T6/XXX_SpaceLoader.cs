// using UnityEngine;

// public class T6SpaceLoader : MonoBehaviour
// {
//     [Header("Prefabs")]
//     public GameObject floorPrefab;
//     public GameObject furniturePrefab;

//     private void Start()
//     {
//         if (T6LoadedSpaceCache.Detail != null)
//         {
//             Debug.Log($"[Loader] Detail 수신: floors={T6LoadedSpaceCache.Detail.floors?.Count}, furnitures={T6LoadedSpaceCache.Detail.furnitures?.Count}");
//             Load(T6LoadedSpaceCache.Detail);
//         }
//         else
//         {
//             Debug.LogWarning("[Loader] Cache.Detail 없음");
//         }
//     }

//     public void Load(T6SpaceDetail detail)
//     {
//         if (detail == null)
//         {
//             Debug.LogError("[Loader] detail == null");
//             return;
//         }

//         ClearScene();

//         // Floors
//         if (detail.floors != null)
//         {
//             foreach (var f in detail.floors)
//             {
//                 var go = Instantiate(floorPrefab);
//                 var piece = go.GetComponent<FloorPiece>();
//                 if (piece == null)
//                 {
//                     Debug.LogError("[Loader] floorPrefab에 FloorPiece 컴포넌트 없음");
//                     continue;
//                 }
//                 piece.FromT6Data(f);
//                 RoomManager.Instance.RegisterPiece(piece);
//             }
//         }

//         // Furnitures
//         if (detail.furnitures != null)
//         {
//             foreach (var fu in detail.furnitures)
//             {
//                 var go = Instantiate(furniturePrefab);
//                 var piece = go.GetComponent<FurniturePiece>();
//                 if (piece == null)
//                 {
//                     Debug.LogError("[Loader] furniturePrefab에 FurniturePiece 컴포넌트 없음");
//                     continue;
//                 }

//                 piece.transform.localPosition = fu.position;
//                 piece.transform.localRotation = fu.rotation;
//                 piece.ApplySize(fu.size);
//             }
//         }

//         // (옵션) 상단 이름 UI 갱신
//         var headerUI = FindFirstObjectByType<T6SpaceHeaderUI>();
//         if (headerUI != null)
//             headerUI.SetSpaceDetail(detail);
//     }

//     private void ClearScene()
//     {
//         foreach (var f in FindObjectsByType<FloorPiece>(FindObjectsSortMode.None))
//             Destroy(f.gameObject);

//         foreach (var fu in FindObjectsByType<FurniturePiece>(FindObjectsSortMode.None))
//             Destroy(fu.gameObject);
//     }
// }
