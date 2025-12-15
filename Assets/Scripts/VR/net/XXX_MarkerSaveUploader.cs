// using System.Collections;
// using System.Text;
// using UnityEngine;
// using UnityEngine.Networking;

// /// <summary>
// /// MARKER.json 저장 업로더.
// /// SpaceSaveUploader와 동일 패턴.
// /// 
// /// 흐름:
// /// 1) MarkerDefinitionRepository → MarkerSaveFileDto 수집
// /// 2) JSON 직렬화
// /// 3) Presigned URL 요청
// /// 4) S3 PUT 업로드
// /// </summary>
// public class MarkerSaveUploader : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private SpaceApi spaceApi;

//     [Header("Options")]
//     [SerializeField] private string markerFileName = "MARKER.json";

//     /// <summary>
//     /// 외부(UI 버튼 등)에서 호출
//     /// </summary>
//     public void Save()
//     {
//         StartCoroutine(SaveRoutine());
//     }

//     private IEnumerator SaveRoutine()
//     {
//         // =========================
//         // 1. Environment ID 확인
//         // =========================
//         long envId = LoadedSpaceCache.EnvironmentId;
//         if (envId <= 0)
//         {
//             Debug.LogError("[MarkerSave] EnvironmentId 없음");
//             yield break;
//         }

//         if (spaceApi == null)
//         {
//             Debug.LogError("[MarkerSave] SpaceApi reference missing");
//             yield break;
//         }

//         // =========================
//         // 2. 현재 마커 상태 수집
//         // =========================
//         MarkerSaveFileDto dto = CollectMarkerData();

//         string json = JsonUtility.ToJson(dto, true);
//         byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

//         // =========================
//         // 3. Presigned URL 요청
//         // =========================
//         bool done = false;
//         S3PresignedUrlResponseDto uploadInfo = null;

//         yield return spaceApi.RequestUploadUrl(
//             envId,
//             markerFileName,
//             res =>
//             {
//                 uploadInfo = res;
//                 done = true;
//             },
//             err =>
//             {
//                 Debug.LogError("[MarkerSave] Presigned URL 요청 실패: " + err);
//                 done = true;
//             });

//         if (!done || uploadInfo == null || string.IsNullOrEmpty(uploadInfo.presignedUploadUrl))
//         {
//             Debug.LogError("[MarkerSave] 업로드 URL 없음");
//             yield break;
//         }

//         // =========================
//         // 4. S3 PUT 업로드
//         // =========================
//         using (var req = new UnityWebRequest(uploadInfo.presignedUploadUrl, UnityWebRequest.kHttpVerbPUT))
//         {
//             req.uploadHandler = new UploadHandlerRaw(jsonBytes);
//             req.downloadHandler = new DownloadHandlerBuffer();

//             // Presigned URL은 Content-Type 중요
//             req.SetRequestHeader("Content-Type", "application/json");

//             yield return req.SendWebRequest();

//             if (req.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.LogError("[MarkerSave] S3 업로드 실패: " + req.error);
//                 yield break;
//             }
//         }

//         Debug.Log("[MarkerSave] MARKER.json 업로드 완료");
//     }

//     // =========================
//     // Collect
//     // =========================

//     private MarkerSaveFileDto CollectMarkerData()
//     {
//         var repo = MarkerDefinitionRepository.Instance;
//         var file = new MarkerSaveFileDto();

//         if (repo == null)
//         {
//             Debug.LogError("[MarkerSave] MarkerDefinitionRepository not found");
//             return file;
//         }

//         var all = repo.GetAll();
//         if (all == null)
//             return file;

//         foreach (var def in all)
//         {
//             if (def == null)
//                 continue;

//             var dto = new MarkerDefinitionDto
//             {
//                 id = def.DefinitionId,
//                 name = def.DisplayName,
//                 description = def.Description,
//                 colorIndex = def.ColorIndex,
//                 color = new ColorDto(def.Color),
//                 isFavorite = def.IsFavorite,
//                 placement = def.IsPlaced
//                     ? new MarkerPlacementDto
//                     {
//                         position = def.Placement.position,
//                         rotation = def.Placement.rotation
//                     }
//                     : null
//             };

//             file.markers.Add(dto);
//         }

//         return file;
//     }
// }
