using System.Collections.Generic;
using UnityEngine;

public class MarkerSlotSpawner : MonoBehaviour
{
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

    // 1 Definition = 1 MarkerInstance 규칙을 위한 잠금(락)
    private readonly HashSet<string> lockedDefinitions = new HashSet<string>();  

    public static MarkerSlotSpawner Current { get; private set; }

    private void Awake()
    {
        slotMap = new Dictionary<string, MarkerDefinitionSlot>();

        if (filterController != null)
            filterController.OnFilterChanged += ApplyFilter;

        Current = this;
    }

    /// <summary>
    /// 새 Definition이 생겼을 때 슬롯을 자동 생성한다.
    /// 여기서 슬롯에 (defId, spawner) 참조를 "주입"한다.
    /// </summary>
    public MarkerDefinitionSlot SpawnSlot(string definitionId)
    {
        var slot = Instantiate(slotPrefab, slotRoot);
        slot.Initialize(definitionId, this, infoPanel);
        slotMap[definitionId] = slot;
        return slot;
    }

    /// <summary>
    /// 슬롯이 “배치 시작”을 요청하면 호출된다.
    /// 실제 MarkerInstance 생성 및 BeginPlaceNew 호출은 여기서 한다.
    /// </summary>
    public void BeginPlacement(string definitionId)
    {
        if (IsDefinitionLocked(definitionId))
            return;

        // Definition 존재 확인 (안전)
        var repo = MarkerDefinitionRepository.Instance;
        if (repo == null || repo.GetById(definitionId) == null)
        {
            Debug.LogError($"[MarkerSlotSpawner] Definition not found: {definitionId}");
            return;
        }

        // 1 Definition = 1 Instance 잠금
        lockedDefinitions.Add(definitionId);

        // 마커 인스턴스 생성 + 초기화 + 배치 시작
        var instance = Instantiate(markerPrefab);
        instance.Initialize(definitionId);
        MarkerInstanceRegistry.Instance.Register(instance);
        // (선택) 인벤토리 스크롤 차단
        if (InventoryScroll.Instance != null)
            InventoryScroll.Instance.SetScroll(false);

        // 이동 컨트롤러로 배치 진입
        moveController.BeginPlaceNew(instance);
    }

    public bool IsDefinitionLocked(string definitionId)
        => lockedDefinitions.Contains(definitionId);

    /// <summary>
    /// 배치 실패/취소/삭제 등으로 다시 발사 가능하게 만들 때 호출
    /// (MoveController 쪽에서 상황에 맞게 호출해줘야 함)
    /// </summary>
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
        // Debug.Log("Call ApplyFilter");
        var registry = MarkerInstanceRegistry.Instance;
        var repo = MarkerDefinitionRepository.Instance;

        List<MarkerInstance> rotateTargets = new List<MarkerInstance>();
        if (repo == null)
            return;

        bool favoriteOnly = filterController != null && filterController.FavoriteOnly;
        string keyword = filterController != null
            ? filterController.SearchKeyword
            : string.Empty;

        keyword = keyword?.ToLowerInvariant();

        foreach (var pair in slotMap)
        {
            string defId = pair.Key;
            MarkerDefinitionSlot slot = pair.Value;

            if (slot == null)
                continue;

            var def = repo.GetById(defId);
            if (def == null)
            {
                slot.gameObject.SetActive(false);
                continue;
            }

            bool visible = true;

            // 즐겨찾기 필터
            if (favoriteOnly && !def.IsFavorite)
                visible = false;

            // 검색 필터
            if (visible && !string.IsNullOrEmpty(keyword))
            {
                string name = def.DisplayName?.ToLowerInvariant() ?? string.Empty;
                if (!name.Contains(keyword))
                    visible = false;
            }

            slot.gameObject.SetActive(visible);

            if (visible && (favoriteOnly ||!string.IsNullOrEmpty(keyword)))
            {
                // 배치된 정의만 회전 대상으로
                if (def.IsPlaced && registry != null)
                {
                    var instance = registry.Get(def.DefinitionId);
                    if (instance != null)
                    {
                        rotateTargets.Add(instance);
                    }
                }
            }
        }
        if (MarkerRotateAnimator.Instance != null)
        {
            Debug.Log(rotateTargets.Count);
            if (rotateTargets.Count > 0)
                MarkerRotateAnimator.Instance.SetMultipleTargets(rotateTargets);
            else
                MarkerRotateAnimator.Instance.StopRotate();
        }
    }


}
