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

        string accessToken = "eyJraWQiOiJFQk1jMXlEaXVOQTlsNTIwd00wK2VqZTk2RmxtN2JJS0lzUm1VOXhheGJBPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI1NGM4YmQ2Yy02MDMxLTcwY2UtYWRjMS03ZGM3ZDcwOTRjNjMiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtbm9ydGhlYXN0LTIuYW1hem9uYXdzLmNvbVwvYXAtbm9ydGhlYXN0LTJfWXBTMHpwMDlLIiwiY2xpZW50X2lkIjoiM2xyMW1zcGJtYzZwcmU4amtyaWZjMGFqajYiLCJvcmlnaW5fanRpIjoiZjcyM2U1NzYtYmI3ZS00NTVjLWE2MmItMTVmNzRkNzkwNjdmIiwiZXZlbnRfaWQiOiI1ZDAwYWZlYy1lM2M0LTQzN2ItYjk4MC1kOGJhYjlkNzUwZWUiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIiwiYXV0aF90aW1lIjoxNzYzOTIyMDUyLCJleHAiOjE3NjM5MjU2NTIsImlhdCI6MTc2MzkyMjA1MiwianRpIjoiZjgxODMyNjUtYmM4Yy00NmVhLTg2MDQtYTgwMzE0OWRiZDRlIiwidXNlcm5hbWUiOiJkZXZ0NmVAZ21haWwuY29tIn0.trcwSceGxFCXoLN0uKlDvQXW74GaSvWrF2ifNZvPi41ci_9aI5TXgVmppIzmRMiX8C-EDK1jn7Fv8jeNrJOWnxD_B4ocb3I0mpT5NPeslNr_CKu3SHgWJcbgGK6POj-XbNXYnKjXDLn-SRv8wSV9PKSAE4deQTwUKILH7lnota0L-Xsdr5YoySXrB399pHfLAgija6Ny9CnNjgcQt_22Ucq3b5pVV3Ny5fbZsMm91rbV8txqoL5aig1Bq1lNMQE7LOGnxAUwR6UqheTl3RM82ikifuT4PJUHfs8xvemPVFcKyl45eNZUOhr0n-IGu5syIDpEpP_GBWFph-H3NS-STQ";
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

        modelParent.localScale = new Vector3(3f, 3f, 3f);
        modelParent.transform.rotation = Quaternion.Euler(0, -90, 0);

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