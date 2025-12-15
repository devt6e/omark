using UnityEngine;

/// <summary>
/// 현재 씬의 상태를 Manager 기준으로 수집하여
/// SpaceSaveFileDto로 변환하는 저장 전용 수집기.
/// </summary>
public class SpaceSaveCollector : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private FurnitureManager furnitureManager;

    /// <summary>
    /// 현재 공간 상태를 SpaceSaveFileDto로 수집
    /// </summary>
    public SpaceSaveFileDto Collect()
    {
        var dto = new SpaceSaveFileDto();

        CollectMeta(dto);
        CollectFloors(dto);
        CollectFurnitures(dto);

        return dto;
    }

    // =========================
    // Meta
    // =========================

    private void CollectMeta(SpaceSaveFileDto dto)
    {
        // 1️⃣ 기존 SpaceData가 있다면 meta 유지 (편집 중 재저장)
        if (LoadedSpaceCache.SpaceData != null &&
            !string.IsNullOrEmpty(LoadedSpaceCache.SpaceData.meta?.name))
        {
            dto.meta.name = LoadedSpaceCache.SpaceData.meta.name;
        }
        // 2️⃣ 메인화면 요약 정보에서 이름 가져오기
        else if (LoadedSpaceCache.Summary != null &&
                !string.IsNullOrEmpty(LoadedSpaceCache.Summary.name))
        {
            dto.meta.name = LoadedSpaceCache.Summary.name;
        }
        // 3️⃣ 완전 신규 공간
        else
        {
            dto.meta.name = "Untitled Space";
        }

        // 설명은 아직 사용하지 않으므로 빈 값
        dto.meta.description = string.Empty;
    }

    // =========================
    // Floors (RoomManager 기준)
    // =========================

    private void CollectFloors(SpaceSaveFileDto dto)
    {
        if (roomManager == null)
        {
            Debug.LogError("[SpaceSaveCollector] RoomManager reference missing");
            return;
        }

        var floors = roomManager.GetAllPieces();
        if (floors == null) return;

        foreach (var piece in floors)
        {
            if (piece == null) continue;

            dto.floors.Add(piece.ToT6Data());
        }
    }

    // =========================
    // Furnitures (FurnitureManager 기준)
    // =========================

    private void CollectFurnitures(SpaceSaveFileDto dto)
    {
        if (furnitureManager == null)
        {
            Debug.LogError("[SpaceSaveCollector] FurnitureManager reference missing");
            return;
        }

        foreach (var piece in furnitureManager.GetAll())
        {
            if (piece == null) continue;

            dto.furnitures.Add(new T6FurnitureData
            {
                id = piece.Id,
                position = piece.transform.localPosition,
                rotation = piece.transform.localRotation,
                size = piece.Size
            });
        }
    }
}
