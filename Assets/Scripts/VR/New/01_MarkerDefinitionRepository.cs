using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MarkerDefinition의 단일 저장소.
/// 판단 로직 없음, 입력/씬/오브젝트 의존 없음.
/// 오직 "정의(Definition) 목록"과 "확정된 결과(Placement)"만 관리한다.
/// </summary>
public class MarkerDefinitionRepository : MonoBehaviour
{
    public static MarkerDefinitionRepository Instance { get; private set; }

    // =========================
    // Internal Storage
    // =========================
    [Header("Definitions (Runtime)")]
    [SerializeField] private List<MarkerDefinition> definitions = new List<MarkerDefinition>();

    // 빠른 조회용 인덱스
    private readonly Dictionary<string, MarkerDefinition> lookup =
        new Dictionary<string, MarkerDefinition>();

    // =========================
    // Unity Lifecycle
    // =========================
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

    // =========================
    // Build / Rebuild
    // =========================

    /// <summary>
    /// 리스트 기반 데이터를 ID 기반 조회 구조로 재구성
    /// (로드 후, 초기화 시 사용)
    /// </summary>
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

    // =========================
    // Query (Read)
    // =========================

    /// <summary>
    /// 모든 마커 정의 반환 (읽기 전용 사용 권장)
    /// </summary>
    public IReadOnlyList<MarkerDefinition> GetAll()
    {
        return definitions;
    }

    /// <summary>
    /// ID로 마커 정의 조회
    /// </summary>
    public MarkerDefinition GetById(string definitionId)
    {
        if (string.IsNullOrEmpty(definitionId))
            return null;

        lookup.TryGetValue(definitionId, out var def);
        return def;
    }

    // =========================
    // Create / Remove
    // =========================

    /// <summary>
    /// 새로운 마커 정의 생성
    /// </summary>
    public MarkerDefinition Create(string displayName, Color color, int colorIndex, string description = "")
    {
        MarkerDefinition def = new MarkerDefinition(displayName, color, colorIndex, description);

        definitions.Add(def);
        lookup.Add(def.DefinitionId, def);

        return def;
    }

    /// <summary>
    /// 마커 정의의 이름 / 설명 수정
    /// </summary>
    public bool UpdateInfo(string definitionId, string displayName, string description, int colorIndex, Color color)
    {
        var def = GetById(definitionId);
        if (def == null)
            return false;

        def.UpdateInfo(displayName, description, colorIndex, color);
        return true;
    }

    /// <summary>
    /// 마커 정의 제거
    /// (배치 여부와 무관, 호출 책임은 외부에 있음)
    /// </summary>
    public bool Remove(string definitionId)
    {
        var def = GetById(definitionId);
        if (def == null)
            return false;

        definitions.Remove(def);
        lookup.Remove(definitionId);
        return true;
    }

    /// <summary>
    /// 마커 정의 삭제 (Remove의 의미 명확화용 별칭)
    /// </summary>
    public bool Delete(string definitionId)
    {
        return Remove(definitionId);
    }

    // =========================
    // Placement Control
    // =========================

    /// <summary>
    /// 배치 결과 기록 (확정 상태만 기록)
    /// 판단/유효성 검사는 외부에서 완료된 상태여야 한다.
    /// </summary>
    public void SetPlacement(string definitionId, Vector3 position, Quaternion rotation)
    {
        var def = GetById(definitionId);
        if (def == null)
            return;

        def.SetPlacement(position, rotation);
    }

    /// <summary>
    /// 배치 해제 (미배치 상태로 전환)
    /// </summary>
    public void ClearPlacement(string definitionId)
    {
        var def = GetById(definitionId);
        if (def == null)
            return;

        def.ClearPlacement();
    }

    /// <summary>
    /// 즐겨찾기 상태 설정
    /// </summary>
    public bool SetFavorite(string definitionId, bool favorite)
    {
        var def = GetById(definitionId);
        if (def == null)
            return false;

        def.SetFavorite(favorite);
        return true;
    }


    // =========================
    // Utility
    // =========================

    /// <summary>
    /// 현재 배치된 모든 마커 정의 반환
    /// (씬 로드 시 인스턴스 생성 등에 사용)
    /// </summary>
    public IEnumerable<MarkerDefinition> GetPlacedDefinitions()
    {
        foreach (var def in definitions)
        {
            if (def != null && def.IsPlaced)
                yield return def;
        }
    }

    public void GetDefInfo(string id)
    {
        MarkerDefinition def = GetById(id);
        Debug.Log(
            $"id : {def.DefinitionId} \nname : {def.DisplayName} \ncolor : {def.Color} \n isFavorite : {def.IsFavorite} ");
    }
}
