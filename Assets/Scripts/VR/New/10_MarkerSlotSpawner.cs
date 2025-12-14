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

    // 1 Definition = 1 MarkerInstance 규칙을 위한 잠금(락)
    private readonly HashSet<string> lockedDefinitions = new HashSet<string>();
    private Dictionary<string, MarkerDefinitionSlot> slotMap;

    private void Awake()
    {
        slotMap = new Dictionary<string, MarkerDefinitionSlot>();
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

}
