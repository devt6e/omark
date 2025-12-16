using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using GLTFast;
using System.Threading.Tasks;

public class MarkerAICustom : MonoBehaviour
{
    public static MarkerAICustom Instance{get; private set;}
    [Header("UI")]
    public Button uploadButton;
    public Image previewImage;
    public GameObject restrictionPanel;
    public Button restrictionOkay;

    [Header("Model Parent")]
    public Transform modelParent;

    [Header("Server")]
    public string serverUrl;
    
    private string accessToken;
    private string savedImagePath;
    private string glbPath;

    public Transform GetCustom() => modelParent;
    private void Start()
    {
        uploadButton.onClick.AddListener(OnClickCreateCustomMarker);
        restrictionOkay.onClick.AddListener(OnClickOkay);
        accessToken = PlayerPrefs.GetString("ACCESS_TOKEN", null);
        Instance = this;
    }

    private void OnClickCreateCustomMarker()
    {
        if(CustomMarkerManager.Instance.isFirst)
        {
            CustomMarkerManager.Instance.isFirst = false;
            CustomMarkerManager.Instance.ReplaceCustomMarker();
            PickImageFromGallery();
        }
        else
        {
            restrictionPanel.SetActive(true);
        }
    }

    private void PickImageFromGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path))
                return;

            savedImagePath = ImageCopyUtil.CopyToPersistentPath(path, "custom_marker_icon.png");

            Texture2D tex = NativeGallery.LoadImageAtPath(savedImagePath, 1024);
            if (tex == null)
                return;

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            if (previewImage != null)
            {
                previewImage.sprite = sprite;
                previewImage.preserveAspect = true;
            }

            CustomMarkerManager.Instance.UpdateCustomSlotImage(sprite);

            StartCoroutine(UploadImageAndRequestGLB(savedImagePath));

        }, "이미지를 선택하세요");
    }

    IEnumerator UploadImageAndRequestGLB(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        string fileName = Path.GetFileName(imagePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, fileName, "image/png");

        UnityWebRequest request = UnityWebRequest.Post(serverUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("AI 마커 업로드 실패: " + request.error);
            yield break;
        }

        string glbPath = Path.Combine(Application.persistentDataPath, "custom_marker.glb");
        File.WriteAllBytes(glbPath, request.downloadHandler.data);

        Debug.Log("GLB 저장 완료: " + glbPath);
        MarkerDefinitionRepository.Instance.SetCustomMarkerAssets(CustomMarkerManager.Instance.GetDefID, savedImagePath, glbPath);
        // 👉 GLB 로드는 async 메서드로 분리
        LoadGlbAsync(glbPath);
    }

    private async void LoadGlbAsync(string glbPath)
    {
        // 기존 모델 제거
        foreach (Transform child in modelParent)
            Destroy(child.gameObject);

        var gltf = new GltfImport();

        bool loaded = await gltf.Load(glbPath);
        if (!loaded)
        {
            Debug.LogError("GLB 로드 실패");
            return;
        }

        bool instantiated = await gltf.InstantiateMainSceneAsync(modelParent);
        if (!instantiated)
        {
            Debug.LogError("GLB 인스턴스 실패");
            return;
        }

        // 위치 보정
        modelParent.transform.GetChild(0).localPosition = new Vector3(0.1f, -0.15f, -0.15f);
        modelParent.transform.GetChild(0).localScale = Vector3.one * 0.4f;
        modelParent.transform.GetChild(0).localRotation = Quaternion.Euler(0, -90f, 0);

        Debug.Log("커스텀 마커 GLB 표시 완료");
    }

    public void OnClickOkay()
    {
        restrictionPanel.SetActive(false);
    }
}
