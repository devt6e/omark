using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpaceSaveFileDto를 기반으로
/// FloorPiece / FurniturePiece를 생성하고 초기화하는 Builder.
/// - FloorPiece / FurniturePiece 구조 수정 없음
/// - DTO → 런타임 오브젝트 연결 전담
/// </summary>
public class SpaceBuilder : MonoBehaviour
{
    [Header("Parents")]
    [SerializeField] private Transform floorRoot;
    [SerializeField] private Transform furnitureRoot;

    [Header("Prefabs")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject furniturePrefab;

    // 생성된 오브젝트 추적 (Clear용)
    private readonly List<GameObject> spawnedObjects = new();

    // =========================
    // Public API
    // =========================

    /// <summary>
    /// SPACE.json 데이터로 공간 생성
    /// </summary>
    public void Build(SpaceSaveFileDto data)
    {
        Clear();

        if (data == null)
        {
            Debug.LogError("[SpaceBuilder] SpaceSaveFileDto is null");
            return;
        }

        BuildFloors(data.floors);
        BuildFurnitures(data.furnitures);
    }

    /// <summary>
    /// 빈 공간 생성 (SPACE 파일 없음)
    /// </summary>
    public void BuildEmpty()
    {
        Clear();
        Debug.Log("[SpaceBuilder] Empty space initialized");
    }

    /// <summary>
    /// 기존 생성 오브젝트 제거
    /// </summary>
    public void Clear()
    {
        foreach (var go in spawnedObjects)
        {
            if (go != null)
                Destroy(go);
        }
        spawnedObjects.Clear();
    }

    // =========================
    // Internal Builders
    // =========================

    private void BuildFloors(List<T6FloorData> floors)
    {
        if (floors == null) return;

        foreach (var data in floors)
        {
            var go = Instantiate(floorPrefab, floorRoot);
            go.name = $"Floor_{data.id}";

            var piece = go.GetComponent<FloorPiece>();
            if (piece == null)
            {
                Debug.LogError("[SpaceBuilder] FloorPrefab에 FloorPiece 컴포넌트가 없습니다.");
                Destroy(go);
                continue;
            }

            // DTO → FloorPiece 주입
            piece.FromT6Data(data);

            spawnedObjects.Add(go);
        }
    }

    private void BuildFurnitures(List<T6FurnitureData> furnitures)
    {
        if (furnitures == null) return;

        foreach (var data in furnitures)
        {
            var go = Instantiate(furniturePrefab, furnitureRoot);
            go.name = $"Furniture_{data.id}";

            var piece = go.GetComponent<FurniturePiece>();
            if (piece == null)
            {
                Debug.LogError("[SpaceBuilder] FurniturePrefab에 FurniturePiece 컴포넌트가 없습니다.");
                Destroy(go);
                continue;
            }

            // ===== FurniturePiece 기준 초기화 순서 =====
            // 1. 위치
            piece.transform.localPosition = data.position;

            // 2. 회전 (Quaternion 그대로 사용)
            piece.transform.localRotation = data.rotation;

            // 3. 사이즈 적용 (Collider 포함)
            piece.Initialize(data.size);

            spawnedObjects.Add(go);
        }
    }
}
