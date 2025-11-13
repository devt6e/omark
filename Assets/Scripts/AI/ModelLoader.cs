// using UnityEngine;
// using UnityEngine.Networking;
// using System.Collections;
// using GLTFast;

// public class ModelLoader : MonoBehaviour
// {
//     public string modelUrl = "http://your-ec2-server/uploads/output.glb";

//     IEnumerator Start()
//     {
//         UnityWebRequest www = UnityWebRequest.Get(modelUrl);
//         yield return www.SendWebRequest();

//         if (www.result == UnityWebRequest.Result.Success)
//         {
//             byte[] modelData = www.downloadHandler.data;
//             var path = System.IO.Path.Combine(Application.persistentDataPath, "model.glb");
//             System.IO.File.WriteAllBytes(path, modelData);

//             Debug.Log("✅ 모델 파일 저장 완료: " + path);

//             // GLTF 로드 (코루틴 방식)
//             var gltf = new GltfImport();
//             bool success = false;

//             // GLTFast는 Load(path) 대신 Load(path, ...)를 사용
//             // settings 매개변수와 instantiator 매개변수를 명시적으로 지정
//             yield return gltf.Load(
//                 path: path, 
//                 settings: new ImportSettings(), 
//                 instantiator: new GameObjectInstantiator(gltf, gameObject.transform));
//             success = gltf.Loaded;

//             if (success)
//             {
//                 Debug.Log("✅ 3D 모델 로드 완료!");
//             }
//             else
//             {
//                 Debug.LogError("❌ GLTF 로드 실패!");
//             }
//         }
//         else
//         {
//             Debug.LogError("모델 다운로드 실패: " + www.error);
//         }
//     }
// }
