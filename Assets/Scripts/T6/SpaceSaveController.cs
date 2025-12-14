using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class T6SpaceSaveController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject confirmPopup;
    [SerializeField] private GameObject resultPopup;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button btnSave;
    [SerializeField] private TMP_Text txtSpaceName;

    [Header("Logic")]
    [SerializeField] private T6SpaceSaver saver;
    [SerializeField] private ApiClient apiClient;
    [SerializeField] private SpaceApi spaceApi;

    private void Start()
    {
        btnSave.onClick.AddListener(OnClickSave);
    }

    private void OnClickSave()
    {
        StartCoroutine(SaveFlow());
    }

    private IEnumerator SaveFlow()
    {
        long envId = T6LoadedSpaceCache.EnvironmentId;
        if (envId <= 0)
        {
            ShowError("Environment ID 없음");
            yield break;
        }

        string spaceName = txtSpaceName.text;
        string json = saver.BuildJson(spaceName);

        // 1. Presigned URL 요청
        S3PresignedUrlResponseDto presigned = null;

        yield return spaceApi.RequestUploadUrl(
            envId,
            "space_detail.json",
            onSuccess: dto => presigned = dto,
            onError: msg => ShowError($"Presigned URL 요청 실패\n{msg}")
        );

        if (presigned == null || string.IsNullOrEmpty(presigned.presignedUploadUrl))
        {
            ShowError("Presigned URL 수신 실패");
            yield break;
        }

        // 2. S3 업로드
        yield return UploadToS3(
            presigned.presignedUploadUrl,
            json,
            onFail: err => ShowError($"S3 업로드 실패\n{err}")
        );

        // 3. 환경 이름 업데이트
        yield return spaceApi.UpdateEnvironment(
            envId,
            spaceName,
            onSuccess: () =>
            {
                Debug.Log("[Saver] 저장 완료");
                ShowSuccess();
            },
            onError: msg => ShowError($"환경 이름 업데이트 실패\n{msg}")
        );
    }

    private IEnumerator UploadToS3(string uploadUrl, string json, System.Action<string> onFail)
    {
        using UnityWebRequest req = new UnityWebRequest(uploadUrl, "PUT");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("[Saver] S3 업로드 시작");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.error);
            onFail?.Invoke(req.error);
        }
        else
        {
            Debug.Log("[Saver] S3 업로드 성공");
        }
    }

    // ---------------- UI ----------------

    private void ShowError(string message)
    {
        Debug.LogError("[Saver] " + message);
        resultText.text = $"저장에 실패했습니다.\n{message}";
        confirmPopup.SetActive(false);
        resultPopup.SetActive(true);
    }

    private void ShowSuccess()
    {
        resultText.text = "저장이 완료되었습니다.";
        confirmPopup.SetActive(false);
        resultPopup.SetActive(true);
    }
}
