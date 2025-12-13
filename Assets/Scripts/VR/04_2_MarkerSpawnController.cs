// using UnityEngine;

// public class MarkerSpawnController : MonoBehaviour
// {
//     [Header("Prefab")]
//     [SerializeField] private MarkerEntity markerPrefab;

//     [Header("Refs")]
//     [SerializeField] private MarkerMoveController moveController;

//     public MarkerEntity SpawnFromLaunchPad(
//         UIMarkerLaunchPad launchPad,
//         T6MarkerData template
//     )
//     {
//         // 데이터 생성 (템플릿 복사)
//         T6MarkerData data = new T6MarkerData(
//             template.displayName,
//             template.color
//         );

//         MarkerEntity marker = Instantiate(markerPrefab);
//         marker.Initialize(data);

//         // 이동 컨트롤러에 PreviewPlacing 요청
//         moveController.BeginPlaceNew(marker, launchPad);

//         return marker;
//     }
// }
