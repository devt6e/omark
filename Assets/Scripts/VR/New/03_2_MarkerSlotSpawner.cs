using System.Collections.Generic;
using UnityEngine;

public class MarkerSlotSpawner : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private Transform markerRoot;

    [Header("UI")]
    [SerializeField] private Transform slotRoot;
    [SerializeField] private MarkerDefinitionSlot slotPrefab;

    [Header("Placement Refs (Scene Objects)")]
    [SerializeField] private MarkerInstance markerPrefab;
    [SerializeField] private MarkerMoveController moveController;

    [SerializeField] private MarkerInfoPanel infoPanel;
    [SerializeField] private MarkerFilterController filterController;

    [Header("Slot Map(Runtime)")]
    [SerializeField] private Dictionary<string, MarkerDefinitionSlot> slotMap;

    private readonly HashSet<string> lockedDefinitions = new HashSet<string>();

    public static MarkerSlotSpawner Current { get; private set; }
    private bool isFirstCustom = true;

    private void Awake()
    {
        slotMap = new Dictionary<string, MarkerDefinitionSlot>();

        if (filterController != null)
            filterController.OnFilterChanged += ApplyFilter;

        Current = this;
    }

    /// <summary>
    /// ✅ 로드 후: Repository 기준으로 슬롯을 통째로 재생성한다.
    /// - 배치된 정의는 잠금 처리(1 Definition = 1 Instance 규칙)
    /// </summary>
    public void BuildAllFromRepository()
    {
        ClearSlots();
        lockedDefinitions.Clear();

        var repo = MarkerDefinitionRepository.Instance;
        if (repo == null) return;

        var all = repo.GetAll();
        if (all == null) return;

        for (int i = 0; i < all.Count; i++)
        {
            var def = all[i];
            if (def == null || string.IsNullOrEmpty(def.DefinitionId))
                continue;

            SpawnSlot(def.DefinitionId);

            // 배치된 마커는 이미 인스턴스가 복원될 것이므로 슬롯 잠금
            if (def.IsPlaced)
                lockedDefinitions.Add(def.DefinitionId);
        }

        // 로드 직후 필터 반영
        ApplyFilter();
    }

    private void ClearSlots()
    {
        if (slotRoot != null)
        {
            for (int i = slotRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(slotRoot.GetChild(i).gameObject);
            }
        }
        slotMap.Clear();
    }

    public MarkerDefinitionSlot SpawnSlot(string definitionId)
    {
        var slot = Instantiate(slotPrefab, slotRoot);
        slot.Initialize(definitionId, this, infoPanel);
        slotMap[definitionId] = slot;
        return slot;
    }

    public void BeginPlacement(string definitionId)
    {
        if (IsDefinitionLocked(definitionId))
            return;

        var repo = MarkerDefinitionRepository.Instance;
        if (repo == null || repo.GetById(definitionId) == null)
        {
            Debug.LogError($"[MarkerSlotSpawner] Definition not found: {definitionId}");
            return;
        }

        lockedDefinitions.Add(definitionId);
        Debug.Log($"spawner : {repo.GetById(definitionId).IsCustomized}");
        var instance = Instantiate(markerPrefab);
        if (repo.GetById(definitionId).IsCustomized && isFirstCustom)
        {
            Debug.Log("first custom");
            foreach (Transform child in instance.transform)
                Destroy(child.gameObject);
            MarkerAICustom.Instance.GetCustom().SetParent(instance.transform, false);
            isFirstCustom = false;
        }
        instance.transform.SetParent(markerRoot, false);
        instance.Initialize(definitionId);
        MarkerInstanceRegistry.Instance.Register(instance);

        if (InventoryScroll.Instance != null)
            InventoryScroll.Instance.SetScroll(false);

        moveController.BeginPlaceNew(instance);
    }

    public bool IsDefinitionLocked(string definitionId)
        => lockedDefinitions.Contains(definitionId);

    public void UnlockDefinition(string definitionId)
    {
        lockedDefinitions.Remove(definitionId);

        if (InventoryScroll.Instance != null)
            InventoryScroll.Instance.SetScroll(true);
    }

    public void RefreshSlot(string definitionId)
    {
        if (slotMap == null)
        {
            Debug.LogError("[SlotSpawner] slotMap is null");
            return;
        }

        if (!slotMap.TryGetValue(definitionId, out var slot) || slot == null)
        {
            Debug.LogWarning($"[SlotSpawner] RefreshSlot failed. id={definitionId}");
            return;
        }

        slot.Refresh();
    }

    public void RemoveSlot(string definitionId)
    {
        if (slotMap == null)
        {
            Debug.LogError("[SlotSpawner] slotMap is null");
            return;
        }

        if (!slotMap.TryGetValue(definitionId, out var slot) || slot == null)
        {
            Debug.LogWarning($"[SlotSpawner] RemoveSlot failed. id={definitionId}");
            return;
        }

        slotMap.Remove(definitionId);
        Destroy(slot.gameObject);
    }

    private void ApplyFilter()
    {
        var registry = MarkerInstanceRegistry.Instance;
        var repo = MarkerDefinitionRepository.Instance;

        List<MarkerInstance> rotateTargets = new List<MarkerInstance>();
        if (repo == null)
            return;

        bool favoriteOnly = filterController != null && filterController.FavoriteOnly;
        string keyword = filterController != null ? filterController.SearchKeyword : string.Empty;
        keyword = keyword?.ToLowerInvariant();

        foreach (var pair in slotMap)
        {
            string defId = pair.Key;
            MarkerDefinitionSlot slot = pair.Value;
            if (slot == null) continue;

            var def = repo.GetById(defId);
            if (def == null)
            {
                slot.gameObject.SetActive(false);
                continue;
            }

            bool visible = true;

            if (favoriteOnly && !def.IsFavorite)
                visible = false;

            if (visible && !string.IsNullOrEmpty(keyword))
            {
                string name = def.DisplayName?.ToLowerInvariant() ?? string.Empty;
                if (!name.Contains(keyword))
                    visible = false;
            }

            slot.gameObject.SetActive(visible);

            if (visible && (favoriteOnly || !string.IsNullOrEmpty(keyword)))
            {
                if (def.IsPlaced && registry != null)
                {
                    var instance = registry.Get(def.DefinitionId);
                    if (instance != null)
                        rotateTargets.Add(instance);
                }
            }
        }

        if (MarkerRotateAnimator.Instance != null)
        {
            if (rotateTargets.Count > 0)
                MarkerRotateAnimator.Instance.SetMultipleTargets(rotateTargets);
            else
                MarkerRotateAnimator.Instance.StopRotate();
        }
    }
}
