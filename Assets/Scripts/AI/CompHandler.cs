// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using UnityEngine.Networking;
// using System.IO;
// using GLTFast; // gltFast import

// public class CompHandler : MonoBehaviour
// {
//     [Header("UI References")]
//     public RawImage displayImage;
//     public Text feedbackText;

//     [Header("3D Model Root")]
//     public Transform modelParent; // 모델을 배치할 부모 Transform

//     private Texture2D loadedTexture;
//     private const string HF_API_URL = "https://api-inference.huggingface.co/models/stabilityai/TripoSR";
//     private const string HF_API_KEY = "YOUR_HF_API_KEY"; // ⚠️ 실제 키로 교체

//     public void PickImage()
//     {
//         NativeGallery.GetImageFromGallery((path) =>
//         {
//             if (path == null)
//             {
//                 ShowFeedback("이미지 선택이 취소되었습니다.", Color.gray);
//                 return;
//             }

//             Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 1024);
//             if (texture == null)
//             {
//                 ShowFeedback("이미지를 불러올 수 없습니다.", Color.red);
//                 return;
//             }

//             loadedTexture = texture;
//             displayImage.texture = loadedTexture;
//             displayImage.color = Color.white;

//             ShowFeedback("이미지가 로드되었습니다. 3D 변환 중...", Color.cyan);
//             StartCoroutine(SendToTripoSR(path));
//         },
//         "이미지를 선택하세요",
//         "image/*");
//     }

//     private IEnumerator SendToTripoSR(string imagePath)
//     {
//         byte[] imageData = File.ReadAllBytes(imagePath);

//         UnityWebRequest req = new UnityWebRequest(HF_API_URL, "POST");
//         req.uploadHandler = new UploadHandlerRaw(imageData);
//         req.downloadHandler = new DownloadHandlerBuffer();
//         req.SetRequestHeader("Authorization", "Bearer " + HF_API_KEY);
//         req.SetRequestHeader("Content-Type", "application/octet-stream");

//         yield return req.SendWebRequest();

//         if (req.result != UnityWebRequest.Result.Success)
//         {
//             ShowFeedback("3D 변환 실패: " + req.error, Color.red);
//             yield break;
//         }

//         byte[] glbData = req.downloadHandler.data;
//         if (glbData == null || glbData.Length == 0)
//         {
//             ShowFeedback("GLB 데이터가 비어 있습니다.", Color.red);
//             yield break;
//         }

//         ShowFeedback("3D 모델 로드 중...", Color.yellow);
//         yield return LoadGLBFromMemory(glbData);
//     }

//     private IEnumerator LoadGLBFromMemory(byte[] glbData)
//     {
//         // gltFast 로더 생성
//         var gltf = new GltfImport();
//         var success = await gltf.LoadGltfBinary(glbData, new ImportSettings());

//         if (success)
//         {
//             if (modelParent.childCount > 0)
//                 Destroy(modelParent.GetChild(0).gameObject);

//             var instantiator = new GameObjectInstantiator(gltf, modelParent);
//             gltf.InstantiateMainScene(instantiator);

//             ShowFeedback("3D 모델 로드 완료!", new Color(0.2f, 0.8f, 0.2f));
//         }
//         else
//         {
//             ShowFeedback("모델 로딩 실패", Color.red);
//         }

//         yield return null;
//     }

//     private void ShowFeedback(string message, Color color)
//     {
//         if (feedbackText != null)
//         {
//             feedbackText.text = message;
//             feedbackText.color = color;
//             StopAllCoroutines();
//             StartCoroutine(ClearFeedbackAfterDelay(3f));
//         }
//     }

//     private IEnumerator ClearFeedbackAfterDelay(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         if (feedbackText != null)
//             feedbackText.text = "";
//     }
// }
