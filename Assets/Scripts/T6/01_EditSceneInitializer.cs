using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 편집화면 진입 시 공간을 완전히 초기화하는 엔트리 포인트.
/// 흐름:
/// 1) 캐시 검증
/// 2) 서버에서 공간 메타 최신화
/// 3) SPACE 파일 URL 확보
/// 4) S3에서 SPACE.json 다운로드
/// 5) 역직렬화
/// 6) 공간 오브젝트 생성
/// </summary>
public class EditSceneInitializer : MonoBehaviour
{
    [Header("API")]
    [SerializeField] private SpaceApi spaceApi;

    // [Header("Space Builder")]
    [SerializeField] private SpaceBuilder spaceBuilder; 
    // SPACE DTO → 실제 바닥/벽/가구 생성 담당 (이미 존재한다고 가정)

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        // =========================
        // 1. 캐시 검증
        // =========================
        if (!LoadedSpaceCache.HasEnvironment)
        {
            Debug.LogError("[EditInit] 선택된 공간 캐시가 없습니다.");
            yield break;
        }

        long envId = LoadedSpaceCache.EnvironmentId;

        // =========================
        // 2. 서버에서 최신 공간 메타 조회
        // =========================
        VirtualEnvironmentResponseDto envDto = null;
        bool done = false;
        string error = null;

        yield return spaceApi.GetEnvironmentDetail(
            envId,
            data =>
            {
                envDto = data;
                done = true;
            },
            err =>
            {
                error = err;
                done = true;
            });

        if (!done || envDto == null)
        {
            Debug.LogError("[EditInit] 공간 메타 조회 실패: " + error);
            yield break;
        }

        // 캐시 갱신
        // LoadedSpaceCache.Environment = envDto;

        // =========================
        // 3. SPACE 파일 정보 확보
        // =========================
        EnvironmentFileDto spaceFile = null;
        if (envDto.files != null)
            spaceFile = envDto.files.Find(f => f.fileType == "SPACE");

        if (spaceFile == null || string.IsNullOrEmpty(spaceFile.fileUrl))
        {
            Debug.Log("[EditInit] SPACE 파일 없음 → 빈 공간으로 시작");
            // spaceBuilder.BuildEmpty();
            yield break;
        }
        // =========================
        // 4. S3에서 SPACE.json 다운로드
        // =========================
        string json = null;
        yield return DownloadJson(spaceFile.fileUrl, result => json = result);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("[EditInit] SPACE 파일 다운로드 실패");
            yield break;
        }

        // =========================
        // 5. 역직렬화
        // =========================
        SpaceSaveFileDto spaceData = null;

        try
        {
            spaceData = JsonUtility.FromJson<SpaceSaveFileDto>(json);
        }
        catch
        {
            Debug.LogError("[EditInit] SPACE JSON 파싱 실패");
            yield break;
        }

        if (spaceData == null)
        {
            Debug.LogError("[EditInit] SPACE 데이터 비어있음");
            yield break;
        }

        // =========================
        // 6. 공간 생성
        // =========================
        spaceBuilder.Build(spaceData);
        // SPACE.json 로드 + SpaceBuilder 완료 후
        Debug.Log("[EditInit] 공간 초기화 완료");
    }


    // ---------------------------
    // S3 JSON 다운로드 유틸
    // ---------------------------
    private IEnumerator DownloadJson(string url, System.Action<string> done)
    {
        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                done?.Invoke(null);
                yield break;
            }

            done?.Invoke(req.downloadHandler.text);
        }
    }
}
