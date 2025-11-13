// using System.Collections;
// using UnityEngine;
// using UnityEngine.Networking;

// public class ConvertController : MonoBehaviour
// {
//     private const string API_URL = "https://api-inference.huggingface.co/models/stabilityai/TripoSR";
//     private const string API_KEY = "YOUR_HF_API_KEY"; // Hugging Face 토큰

//     public IEnumerator RequestAndDisplay3DModel(string imagePath)
//     {
//         // 이미지 읽기
//         UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(imagePath);
//         yield return imgReq.SendWebRequest();
//         if (imgReq.result != UnityWebRequest.Result.Success)
//         {
//             Debug.LogError("이미지 로드 실패: " + imgReq.error);
//             yield break;
//         }

//         Texture2D tex = DownloadHandlerTexture.GetContent(imgReq);
//         byte[] imgData = tex.EncodeToPNG();

//         // Hugging Face API 요청 준비
//         UnityWebRequest req = new UnityWebRequest(API_URL, "POST");
//         req.uploadHandler = new UploadHandlerRaw(imgData);
//         req.downloadHandler = new DownloadHandlerBuffer();
//         req.SetRequestHeader("Authorization", "Bearer " + API_KEY);
//         req.SetRequestHeader("Content-Type", "application/octet-stream");

//         Debug.Log("TripoSR 요청 중...");
//         yield return req.SendWebRequest();

//         if (req.result != UnityWebRequest.Result.Success)
//         {
//             Debug.LogError("API 요청 실패: " + req.error);
//             yield break;
//         }

//         // GLB 모델 데이터 수신
//         byte[] glbData = req.downloadHandler.data;

//         if (glbData == null || glbData.Length == 0)
//         {
//             Debug.LogError("GLB 데이터가 비어 있음");
//             yield break;
//         }

//         // GLB 파일 파싱 및 표시
//         DisplayGLB(glbData);
//     }

//     private void DisplayGLB(byte[] glbData)
//     {
//         try
//         {
//             GameObject model = UniGLTF.GltfUtility.ImportGLB(glbData);
//             model.transform.position = Vector3.zero;
//             Debug.Log("모델 표시 완료");
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("GLB 파싱 실패: " + ex.Message);
//         }
//     }
// }
