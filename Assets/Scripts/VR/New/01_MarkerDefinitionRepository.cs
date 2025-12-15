using System.Collections.Generic;
using UnityEngine;

public class MarkerDefinitionRepository : MonoBehaviour
{
    public static MarkerDefinitionRepository Instance { get; private set; }

    [Header("Definitions (Runtime)")]
    [SerializeField] private List<MarkerDefinition> definitions = new List<MarkerDefinition>();

    private readonly Dictionary<string, MarkerDefinition> lookup =
        new Dictionary<string, MarkerDefinition>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup.Clear();

        foreach (var def in definitions)
        {
            if (def == null || string.IsNullOrEmpty(def.DefinitionId))
                continue;

            if (!lookup.ContainsKey(def.DefinitionId))
                lookup.Add(def.DefinitionId, def);
        }
    }

    public IReadOnlyList<MarkerDefinition> GetAll()
    {
        return definitions;
    }

    public MarkerDefinition GetById(string definitionId)
    {
        if (string.IsNullOrEmpty(definitionId))
            return null;

        lookup.TryGetValue(definitionId, out var def);
        return def;
    }

    public MarkerDefinition Create(string displayName, Color color, int colorIndex, string description = "")
    {
        MarkerDefinition def = new MarkerDefinition(displayName, color, colorIndex, description);

        definitions.Add(def);
        lookup.Add(def.DefinitionId, def);

        return def;
    }

    public bool UpdateInfo(string definitionId, string displayName, string description, int colorIndex, Color color)
    {
        var def = GetById(definitionId);
        if (def == null)
            return false;

        def.UpdateInfo(displayName, description, colorIndex, color);
        return true;
    }

    public bool Remove(string definitionId)
    {
        var def = GetById(definitionId);
        if (def == null)
            return false;

        definitions.Remove(def);
        lookup.Remove(definitionId);
        return true;
    }

    public bool Delete(string definitionId) => Remove(definitionId);

    public void SetPlacement(string definitionId, Vector3 position, Quaternion rotation)
    {
        var def = GetById(definitionId);
        if (def == null)
            return;

        def.SetPlacement(position, rotation);
    }

    public void ClearPlacement(string definitionId)
    {
        var def = GetById(definitionId);
        if (def == null)
            return;

        def.ClearPlacement();
    }

    public bool SetFavorite(string definitionId, bool favorite)
    {
        var def = GetById(definitionId);
        if (def == null)
            return false;

        def.SetFavorite(favorite);
        return true;
    }

    public IEnumerable<MarkerDefinition> GetPlacedDefinitions()
    {
        foreach (var def in definitions)
        {
            if (def != null && def.IsPlaced)
                yield return def;
        }
    }

    // // =========================
    // // ✅ Load 지원: 통째 교체
    // // =========================
    public void ReplaceAll(List<SpaceMarkerDto> markers)
    {
        definitions.Clear();
        lookup.Clear();

        if (markers == null)
            return;

        foreach (var dto in markers)
        {
            if (dto == null || string.IsNullOrEmpty(dto.id))
                continue;

            MarkerPlacement placement = null;
            if (dto.placement != null)
            {
                placement = new MarkerPlacement(
                    dto.placement.position,
                    dto.placement.rotation
                );
            }

            var def = new MarkerDefinition(
                dto.id,
                dto.name ?? "Marker",
                dto.color,
                dto.colorIndex,
                dto.description ?? "",
                dto.isFavorite,
                placement
            );

            definitions.Add(def);
            lookup[def.DefinitionId] = def;
        }
    }

    /// <summary>
    /// 외부에서 리스트만 넘겨 비우거나 초기화하고 싶을 때 사용.
    /// </summary>
    public void ReplaceAllDefinitions(List<MarkerDefinition> newDefs)
    {
        definitions = newDefs ?? new List<MarkerDefinition>();
        BuildLookup();
    }
}
