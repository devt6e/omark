using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using GLTFast;
using System.Threading.Tasks;

public class ModelUploader : MonoBehaviour
{
    [Header("UI")]
    public Button uploadButton;
    public RawImage previewImage;
    public Transform modelParent;

    [Header("Server")]
    public string serverUrl;

    private string imagePath;

    void Start()
    {
        uploadButton.onClick.AddListener(OnUploadButtonClicked);
    }

    void OnUploadButtonClicked()
    {
        PickImageFromGallery();
    }

    void PickImageFromGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path == null)
            {
                Debug.Log("이미지 선택 취소됨");
                return;
            }

            imagePath = path;

            // 미리보기 로드
            Texture2D tex = NativeGallery.LoadImageAtPath(path, maxSize: 2048);
            previewImage.texture = tex;

            // 서버 업로드
            StartCoroutine(UploadImageToServer(path));

        }, "이미지를 선택하세요");
    }

    IEnumerator UploadImageToServer(string path)
    {
        Debug.Log("이미지 업로드 중...");

        byte[] imageBytes = File.ReadAllBytes(path);
        string fileName = Path.GetFileName(path);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, fileName, "image/png");

        UnityWebRequest request = UnityWebRequest.Post(serverUrl, form);

        string accessToken = "eyJraWQiOiJFQk1jMXlEaXVOQTlsNTIwd00wK2VqZTk2RmxtN2JJS0lzUm1VOXhheGJBPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI1NGM4YmQ2Yy02MDMxLTcwY2UtYWRjMS03ZGM3ZDcwOTRjNjMiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtbm9ydGhlYXN0LTIuYW1hem9uYXdzLmNvbVwvYXAtbm9ydGhlYXN0LTJfWXBTMHpwMDlLIiwiY2xpZW50X2lkIjoiM2xyMW1zcGJtYzZwcmU4amtyaWZjMGFqajYiLCJvcmlnaW5fanRpIjoiMWYyMGNkM2MtMjgzZS00MDZkLWI4MTUtOGIyMTFjNTVlZGVmIiwiZXZlbnRfaWQiOiJjN2M2ZDlhNi01NWNhLTQ1ZGQtYjk0OS02YTk1MGQ1OWM4ZWYiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIiwiYXV0aF90aW1lIjoxNzYzODg0NDAyLCJleHAiOjE3NjM4ODgwMDIsImlhdCI6MTc2Mzg4NDQwMiwianRpIjoiMmEzZWJjZjktZmFmZi00Mjg0LWI0MzQtMDI0YjEyZmY5YzU1IiwidXNlcm5hbWUiOiJkZXZ0NmVAZ21haWwuY29tIn0.c_mk4gnJnZl79XbaR_raT1u4uC_Sr3WhevmRbV-rOwQ2YglMk7HcaCX4lHJZ1RDITr4xQP62aeytDVcSEooYDJOfeyyQMq6GAptt2RS0ENGGCClfWepOJNarBuQw2FTHpBwmbyQMxvcpMtgVSNu8p--_JI2H_pa3DpOluh2ZKv6ks1ac8TFjuB0p76JuH5Sez91oA-oBBHq6wh7bh5M9-DzylOBjGxqzherOvKEuxUAA0T350qjYzie3MIhRWQYe3cswFd72shvDpRfd3xU942IQqFYod2KJL7gxk8yEdQPz8s1V99aiqnLT8ZD1bnUwJ7-wzkpEDxDbsaGtux-Zwg";
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("업로드 실패: " + request.error);
            yield break;
        }

        Debug.Log("GLB 파일 수신 완료!");

        // GLB 파일 저장
        string localGlbPath = Path.Combine(Application.persistentDataPath, "result.glb");
        File.WriteAllBytes(localGlbPath, request.downloadHandler.data);

        Debug.Log("GLB 저장됨: " + localGlbPath);

        // 모델 로드
        yield return LoadGLBModel(localGlbPath);
    }

    public async Task LoadGLBModel(string path)
    {
        // 기존 오브젝트 제거
        foreach (Transform child in modelParent)
            Destroy(child.gameObject);

        var gltf = new GltfImport();

        bool success = await gltf.Load(path);

        if (!success)
        {
            Debug.LogError("GLB 로드 실패!");
            return;
        }

        // 새로운 권장 방식
        bool instantiated = await gltf.InstantiateMainSceneAsync(modelParent);

        modelParent.localScale = new Vector3(10f, 10f, 10f);


        if (!instantiated)
        {
            Debug.LogError("GLB 인스턴스 생성 실패!");
            return;
        }

        Debug.Log("모델 표시 완료!");
    }

    public async void OnClickLoadModel()
    {
        string path = Application.persistentDataPath + "/model.glb";
        await LoadGLBModel(path);
    }
}