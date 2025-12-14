using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class T6SpaceSaveController : MonoBehaviour
{
    [Header("References")]
    public Button btnSave;
    public T6SpaceSaver saver;
    public ApiClient apiClient;     // 서버 HTTP 전송
    public SpaceApi spaceApi;       // 서버 비즈니스 API
    public TMP_Text txtSpaceName;   // 공간 이름 UI

    private void Start()
    {
        btnSave.onClick.AddListener(() => StartCoroutine(SaveFlow()));
    }

    private IEnumerator SaveFlow()
    {
        long envId = T6LoadedSpaceCache.EnvironmentId;
        if (envId <= 0)
        {
            Debug.LogError("Environment ID 없음");
            yield break;
        }
        Debug.Log("[Saver] ENV ID = " + envId);

        // 1) 이름 읽기
        string spaceName = txtSpaceName.text;

        // 2) SpaceDetail → JSON 생성
        string json = saver.BuildJson(spaceName);


        // 3) Presigned URL 요청
        S3PresignedUrlResponseDto presigned = null;

        yield return spaceApi.RequestUploadUrl(
            envId,
            "space_detail.json",
            onSuccess: dto =>
            {
                presigned = dto;
                Debug.Log("[Saver] Presigned URL 획득 성공");
            },
            onError: msg =>
            {
                Debug.LogError("[Saver] Presigned URL 요청 실패: " + msg);
            }
        );

        if (presigned == null)
        {
            Debug.LogError("[Saver] presigned == null");
            yield break;
        }

        // 서버 return
        // presigned.presignedUploadUrl
        // presigned.finalFileUrl
        string uploadUrl = presigned.presignedUploadUrl;
        string finalUrl  = presigned.finalFileUrl;

        if (string.IsNullOrEmpty(uploadUrl))
        {
            Debug.LogError("[Saver] uploadUrl 없음");
            yield break;
        }


        // 4) S3 PUT 업로드
        UnityWebRequest req = new UnityWebRequest(uploadUrl, "PUT");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("[Saver] S3 업로드 시작...");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[Saver] S3 업로드 실패: " + req.error);
            yield break;
        }
        Debug.Log("[Saver] S3 업로드 성공");


        // 5) 서버 환경 이름 업데이트
        yield return spaceApi.UpdateEnvironment(
            envId,
            spaceName,
            onSuccess: () =>
            {
                Debug.Log("[Saver] 환경 이름 업데이트 성공");
                Debug.Log("[Saver] 최종 파일 URL: " + finalUrl);
            },
            onError: (msg) =>
            {
                Debug.LogError("[Saver] 환경 이름 업데이트 실패: " + msg);
            }
        );

        Debug.Log("[Saver] 저장 완료!");
    }
}
