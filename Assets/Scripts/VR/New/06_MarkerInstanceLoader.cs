using UnityEngine;

/// <summary>
/// 씬 시작 시 Repository에 저장된 placement 정보를 기반으로
/// MarkerInstance를 복원하는 로더.
/// 
/// - Definition 중심
/// - Instance는 언제든 재생성 가능
/// - 판단 로직 없음
/// </summary>
public class MarkerInstanceLoader : MonoBehaviour
{
    // =========================
    // Prefab / Refs
    // =========================
    [Header("Prefab")]
    [SerializeField] private MarkerInstance markerPrefab;

    [Header("Optional Parent")]
    [SerializeField] private Transform markerRoot;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Start()
    {
        LoadPlacedMarkers();
    }

    // =========================
    // Load Logic
    // =========================
    private void LoadPlacedMarkers()
    {
        var repo = MarkerDefinitionRepository.Instance;
        if (repo == null)
        {
            Debug.LogError("[MarkerInstanceLoader] MarkerDefinitionRepository not found.");
            return;
        }

        foreach (var def in repo.GetPlacedDefinitions())
        {
            SpawnInstance(def);
        }
    }

    private void SpawnInstance(MarkerDefinition def)
    {
        MarkerInstance instance = markerRoot != null
            ? Instantiate(markerPrefab, markerRoot)
            : Instantiate(markerPrefab);

        instance.Initialize(def.DefinitionId);

        // 확정된 placement 반영
        instance.ApplyPlacement(def.Placement);
    }
}
