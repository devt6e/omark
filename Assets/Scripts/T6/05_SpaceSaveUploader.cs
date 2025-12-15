using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;

/// <summary>
/// SPACE.json 저장을 담당하는 업로더.
/// 흐름:
/// 1) SpaceSaveCollector로 DTO 수집
/// 2) JSON 직렬화
/// 3) Presigned URL 요청
/// 4) S3 PUT 업로드
/// </summary>
public class SpaceSaveUploader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpaceSaveCollector collector;
    [SerializeField] private SpaceApi spaceApi;

    /// <summary>
    /// 저장 시작 (외부 버튼에서 호출)
    /// </summary>
    public void Save()
    {
        StartCoroutine(SaveRoutine());

        
    }

    private IEnumerator SaveRoutine()
    {
        // =========================
        // 1. 환경 ID 확인
        // =========================
        long envId = LoadedSpaceCache.EnvironmentId;
        if (envId <= 0)
        {
            Debug.LogError("[Save] EnvironmentId 없음");
            yield break;
        }

        // =========================
        // 2. 현재 공간 상태 수집
        // =========================
        SpaceSaveFileDto dto = collector.Collect();

        string json = JsonUtility.ToJson(dto, true);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        // =========================
        // 3. Presigned URL 요청
        // =========================
        bool done = false;
        S3PresignedUrlResponseDto uploadInfo = null;

        yield return spaceApi.RequestUploadUrl(
            envId,
            "SPACE.json",
            res =>
            {
                uploadInfo = res;
                done = true;
            },
            err =>
            {
                Debug.LogError("[Save] Presigned URL 요청 실패: " + err);
                done = true;
            });

        if (uploadInfo == null || string.IsNullOrEmpty(uploadInfo.presignedUploadUrl))
        {
            Debug.LogError("[Save] 업로드 URL 없음");
            yield break;
        }

        // =========================
        // 4. S3 PUT 업로드
        // =========================
        using (var req = new UnityWebRequest(uploadInfo.presignedUploadUrl, UnityWebRequest.kHttpVerbPUT))
        {
            req.uploadHandler = new UploadHandlerRaw(jsonBytes);
            req.downloadHandler = new DownloadHandlerBuffer();

            // S3 Presigned URL은 Content-Type 중요
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[Save] S3 업로드 실패: " + req.error);
                yield break;
            }
        }

        Debug.Log("[Save] SPACE.json 업로드 완료");
        SaveToPersistentPath(dto);

        // // =========================
        // // 5. 캐시 메타 갱신 (선택)
        // // =========================
        // if (LoadedSpaceCache.Environment != null)
        // {
        //     // 서버 응답 구조에 따라 URL이 자동 갱신될 수도 있음
        //     Debug.Log("[Save] 저장 완료");
        // }
    }
    private void SaveToPersistentPath(SpaceSaveFileDto spaceData)
    {
        if (spaceData == null)
            return;

        long envId = LoadedSpaceCache.EnvironmentId;

        string path = Path.Combine(
            Application.persistentDataPath,
            $"space_{envId}.json"
        );

        string json = JsonUtility.ToJson(spaceData, true);
        File.WriteAllText(path, json);

        Debug.Log("[Save] SPACE.json saved locally: " + path);
    }
}
