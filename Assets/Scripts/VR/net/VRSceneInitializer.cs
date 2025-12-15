using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// VR 씬 진입 시 공간 + 마커를 초기화하는 엔트리 포인트.
/// 우선순위:
/// 1) 로컬 persistentDataPath (space_{envId}.json)
/// 2) 서버 SPACE.json
/// </summary>
public class VRSceneInitializer : MonoBehaviour
{
    [Header("API")]
    [SerializeField] private SpaceApi spaceApi;

    [Header("Space Builder")]
    [SerializeField] private SpaceBuilder spaceBuilder;

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
            Debug.LogError("[VRInit] 선택된 공간 캐시가 없습니다.");
            yield break;
        }

        long envId = LoadedSpaceCache.EnvironmentId;

        SpaceSaveFileDto spaceData = null;

        // =========================
        // 2. 로컬 파일 우선 로드
        // =========================
        string localPath = Path.Combine(
            Application.persistentDataPath,
            $"space_{envId}.json"
        );

        if (File.Exists(localPath))
        {
            Debug.Log("[VRInit] Load SPACE.json from local file");
            string localJson = File.ReadAllText(localPath);

            try
            {
                spaceData = JsonUtility.FromJson<SpaceSaveFileDto>(localJson);
            }
            catch
            {
                Debug.LogError("[VRInit] Local SPACE.json 파싱 실패");
                yield break;
            }
        }
        else
        {
            Debug.Log("[VRInit] Local file not found → load from server");

            // =========================
            // 3. 서버에서 공간 메타 조회
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
                Debug.LogError("[VRInit] 공간 메타 조회 실패: " + error);
                yield break;
            }

            // =========================
            // 4. SPACE 파일 정보 확보
            // =========================
            EnvironmentFileDto spaceFile = null;
            if (envDto.files != null)
                spaceFile = envDto.files.Find(f => f.fileType == "SPACE");

            if (spaceFile == null || string.IsNullOrEmpty(spaceFile.fileUrl))
            {
                Debug.LogError("[VRInit] SPACE 파일 정보 없음");
                yield break;
            }

            Debug.Log("[VRInit] SPACE file url = " + spaceFile.fileUrl);

            // =========================
            // 5. S3에서 SPACE.json 다운로드
            // =========================
            string json = null;
            yield return DownloadJson(spaceFile.fileUrl, result => json = result);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("[VRInit] SPACE 파일 다운로드 실패");
                yield break;
            }

            Debug.Log("[VRInit] SPACE json length = " + json.Length);

            // =========================
            // 6. 역직렬화
            // =========================
            try
            {
                spaceData = JsonUtility.FromJson<SpaceSaveFileDto>(json);
            }
            catch
            {
                Debug.LogError("[VRInit] SPACE JSON 파싱 실패");
                yield break;
            }

            if (spaceData == null)
            {
                Debug.LogError("[VRInit] SPACE 데이터 비어있음");
                yield break;
            }
        }

        // =========================
        // 7. 공간 생성
        // =========================
        spaceBuilder.Build(spaceData);

        // =========================
        // 8. 마커 적용 (VR 전용)
        // =========================
        ApplyMarkers(spaceData);

        Debug.Log("[VRInit] 공간 + 마커 초기화 완료");
    }

    // ---------------------------
    // Marker Apply (VR 전용)
    // ---------------------------
    private void ApplyMarkers(SpaceSaveFileDto file)
    {
        if (file == null || file.markers == null)
            return;

        var repo = MarkerDefinitionRepository.Instance;
        if (repo == null)
        {
            Debug.LogError("[VRInit] MarkerDefinitionRepository.Instance is null");
            return;
        }

        // 1) Definition 로드
        repo.ReplaceAll(file.markers);

        // 2) 슬롯 UI 생성
        if (MarkerSlotSpawner.Current != null)
            MarkerSlotSpawner.Current.BuildAllFromRepository();

        // 3) 배치된 마커 인스턴스 복원
        var loader = FindAnyObjectByType<MarkerInstanceLoader>();
        if (loader != null)
            loader.LoadPlacedMarkers();
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
