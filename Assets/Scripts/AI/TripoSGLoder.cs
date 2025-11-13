using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using GLTFast;

public class TripoSGLoader : MonoBehaviour
{
    // Hugging Face API Endpoint (Spaces나 모델 이름에 맞게 수정)
    private string huggingFaceUrl = "https://huggingface.co/spaces/VAST-AI/TripoSG/image_to_3d";
    
    // Hugging Face Access Token (개인 토큰)
    private string huggingFaceToken = "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";

    // 유니티 프로젝트 내 이미지 경로 (Assets/images/example.png 등)
    private string imagePath = "Assets/images/example.png";

    // 생성된 모델을 붙일 부모 오브젝트
    public Transform modelParent;

    async void Start()
    {
        Debug.Log("🚀 Tripo-SG 변환 프로세스 시작");

        // 1️⃣ 이미지 읽기
        if (!File.Exists(imagePath))
        {
            Debug.LogError("❌ 이미지 파일이 존재하지 않습니다: " + imagePath);
            return;
        }

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        Debug.Log("📸 이미지 로드 완료 (" + imageBytes.Length + " bytes)");

        // 2️⃣ Hugging Face로 전송 (이미지 → 3D 변환)
        byte[] modelData = await Request3DModelFromHuggingFace(imageBytes);

        if (modelData == null)
        {
            Debug.LogError("❌ Hugging Face로부터 모델 데이터를 받지 못했습니다.");
            return;
        }

        // 3️⃣ 로컬에 저장
        string savePath = Path.Combine(Application.persistentDataPath, "generated_model.glb");
        File.WriteAllBytes(savePath, modelData);
        Debug.Log("💾 모델 파일 저장 완료: " + savePath);

        // 4️⃣ GLTFast로 모델 로드
        await LoadModelAsync(savePath);
    }

    /// <summary>
    /// Hugging Face API에 이미지를 전송하고 3D 모델(.glb) 데이터를 반환받는다.
    /// </summary>
    private async Task<byte[]> Request3DModelFromHuggingFace(byte[] imageBytes)
    {
        using (UnityWebRequest request = new UnityWebRequest(huggingFaceUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(imageBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + huggingFaceToken);
            request.SetRequestHeader("Content-Type", "application/octet-stream");

            Debug.Log("🛰️ Hugging Face에 이미지 전송 중...");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ 요청 실패: " + request.error);
                return null;
            }

            Debug.Log("✅ 모델 데이터 수신 완료 (" + request.downloadHandler.data.Length + " bytes)");
            return request.downloadHandler.data;
        }
    }

    /// <summary>
    /// GLTFast로 로컬 모델 파일을 비동기로 로드하고 Unity에 표시한다.
    /// </summary>
    private async Task LoadModelAsync(string path)
    {
        Debug.Log("📦 GLTFast 로드 시작: " + path);

        var gltf = new GltfImport();
        bool success = await gltf.Load(path);

        if (success)
        {
            gltf.InstantiateMainScene(modelParent != null ? modelParent : transform);
            Debug.Log("✅ 3D 모델 로드 완료!");
        }
        else
        {
            Debug.LogError("❌ 3D 모델 로드 실패!");
        }
    }
}
