using UnityEngine;

public class MarkerInstanceLoader : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private MarkerInstance markerPrefab;

    [Header("Optional Parent")]
    [SerializeField] private Transform markerRoot;

    [Header("Options")]
    [SerializeField] private bool loadOnStart = false;

    private void Start()
    {
        if (loadOnStart)
            LoadPlacedMarkers();
    }

    public void LoadPlacedMarkers()
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
        instance.ApplyPlacement(def.Placement);
        MarkerInstanceRegistry.Instance.Register(instance);
    }
}
