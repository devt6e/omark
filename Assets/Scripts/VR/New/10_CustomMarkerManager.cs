using UnityEngine;

/// <summary>
/// 커스텀 마커 전용 제어자
/// - 기존 슬롯 / 인스턴스 프리팹 그대로 사용
/// - 커스텀 마커만 공간당 1개 제한
/// </summary>
public class CustomMarkerManager : MonoBehaviour
{
    public static CustomMarkerManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MarkerSlotSpawner slotSpawner;
    [SerializeField] private MarkerDefinitionRepository definitionRepo;
    [SerializeField] private MarkerInstanceRegistry instanceRegistry;

    private MarkerDefinitionSlot currentCustomSlot;
    // 현재 커스텀 마커 정의 ID
    private string currentCustomDefinitionId;
    public bool isFirst = true;
    public string GetDefID => currentCustomDefinitionId;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 커스텀 마커 교체 (없으면 생성)
    /// </summary>
    public void ReplaceCustomMarker()
    {
       // 1. Definition 생성
        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.Create(
                "나만의 마커",
                Color.black,
                4,
                "인공지능을 활용한 나만의 마커"
            );

        // 2. 슬롯 자동 생성
        MarkerDefinitionSlot slot =
            slotSpawner.SpawnSlot(def.DefinitionId);

        currentCustomSlot = slot;
        currentCustomDefinitionId = def.DefinitionId;
    }

    public void UpdateCustomSlotImage(Sprite sprite)
    {
        if (currentCustomSlot == null)
            return;

        currentCustomSlot.SetIcon(sprite);
    }

    // =========================
    // State
    // =========================
    public bool HasCustomMarker()
    {
        return !string.IsNullOrEmpty(currentCustomDefinitionId);
    }

//     public void ApplyCustomMarkerModel(string glbLocalPath)
//     {
//         if (string.IsNullOrEmpty(currentCustomDefinitionId))
//             return;

//         MarkerDefinition def =
//             MarkerDefinitionRepository.Instance.GetById(currentCustomDefinitionId);

//         if (def == null)
//             return;

//         // GLB 경로 설정
//         def.ModelPath = glbLocalPath;

//         // 다시 배치 (기존 로직 재사용)
//         MarkerDefinitionSlot slot = slotSpawner.GetSlot(def.DefinitionId);

//         if (slot != null)
//             slot.BeginPlacementFromCode();
// }

}
