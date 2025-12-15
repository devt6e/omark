// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;

// public class MarkerApiClient : MonoBehaviour
// {
//     [Header("Server")]
//     [SerializeField] private string baseUrl = "http://localhost:8080";
//     [Tooltip("예: /api/v1/spaces/{0}/markers (envId가 {0}에 들어감)")]
//     [SerializeField] private string getMarkersPathFormat = "/api/v1/spaces/{0}/markers";

//     [Header("Optional Auth")]
//     [SerializeField] private string bearerToken; // 필요하면 세팅

//     public IEnumerator GetMarkers(long environmentId, Action<bool, List<MarkerDefinitionDto>, string> onDone)
//     {
//         string url = baseUrl.TrimEnd('/') + string.Format(getMarkersPathFormat, environmentId);

//         using (UnityWebRequest req = UnityWebRequest.Get(url))
//         {
//             req.SetRequestHeader("Accept", "application/json");
//             if (!string.IsNullOrEmpty(bearerToken))
//                 req.SetRequestHeader("Authorization", "Bearer " + bearerToken);

//             yield return req.SendWebRequest();

//             if (req.result != UnityWebRequest.Result.Success)
//             {
//                 onDone?.Invoke(false, null, req.error);
//                 yield break;
//             }

//             string json = req.downloadHandler.text;
//             if (string.IsNullOrEmpty(json))
//             {
//                 onDone?.Invoke(false, null, "Empty response body");
//                 yield break;
//             }

//             ApiResponse<List<MarkerDefinitionDto>> res = null;
//             try
//             {
//                 res = JsonUtility.FromJson<ApiResponse<List<MarkerDefinitionDto>>>(json);
//             }
//             catch (Exception e)
//             {
//                 onDone?.Invoke(false, null, "JSON parse error: " + e.Message);
//                 yield break;
//             }

//             if (res == null)
//             {
//                 onDone?.Invoke(false, null, "Response is null");
//                 yield break;
//             }

//             bool ok = string.Equals(res.status, "OK", StringComparison.OrdinalIgnoreCase);
//             if (!ok)
//             {
//                 onDone?.Invoke(false, null, string.IsNullOrEmpty(res.message) ? "Server returned ERROR" : res.message);
//                 yield break;
//             }

//             onDone?.Invoke(true, res.data ?? new List<MarkerDefinitionDto>(), res.message);
//         }
//     }
// }
