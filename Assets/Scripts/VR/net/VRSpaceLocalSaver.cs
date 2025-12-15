using UnityEngine;
using System.IO;

/// <summary>
/// VR 씬 전용 로컬 저장기.
/// - 서버 업로드 ❌
/// - Application.persistentDataPath에 SPACE.json 저장
/// - SpaceSaveCollector 구조 재사용
/// </summary>
public class VRSpaceLocalSaver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpaceSaveCollector collector;

    [Header("Options")]
    [SerializeField] private bool overwriteCache = true;

    /// <summary>
    /// VR 씬에서 저장 버튼 등으로 호출
    /// </summary>
    public void SaveLocal()
    {
        if (collector == null)
        {
            Debug.LogError("[VRLocalSave] SpaceSaveCollector reference missing");
            return;
        }

        long envId = LoadedSpaceCache.EnvironmentId;
        if (envId <= 0)
        {
            Debug.LogError("[VRLocalSave] EnvironmentId 없음");
            return;
        }

        // =========================
        // 1. 현재 공간 상태 수집
        // =========================
        SpaceSaveFileDto dto = collector.Collect();
        if (dto == null)
        {
            Debug.LogError("[VRLocalSave] SpaceSaveFileDto 수집 실패");
            return;
        }

        // =========================
        // 2. 로컬 파일 경로
        // =========================
        string path = Path.Combine(
            Application.persistentDataPath,
            $"space_{envId}.json"
        );

        if (File.Exists(path) && !overwriteCache)
        {
            Debug.Log("[VRLocalSave] Local file exists, overwrite disabled");
            return;
        }

        // =========================
        // 3. 로컬 저장
        // =========================
        string json = JsonUtility.ToJson(dto, true);
        File.WriteAllText(path, json);

        Debug.Log("[VRLocalSave] SPACE.json saved locally: " + path);

        // =========================
        // 4. 런타임 캐시 갱신 (선택)
        // =========================
        LoadedSpaceCache.SpaceData = dto;
    }
}
