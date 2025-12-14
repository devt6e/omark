using UnityEngine;

/// <summary>
/// MarkerDefinition을 공간에 표현하는 인스턴스.
/// - 데이터 소유 ❌
/// - pad/definition ID만 보유
/// - 상태(Preview / Selected / Placed)는 "표현"의 문제
/// </summary>
[RequireComponent(typeof(Collider))]
public class MarkerInstance : MonoBehaviour
{
    // =========================
    // Identity
    // =========================
    [Header("Identity")]
    [SerializeField] private string definitionId;

    // =========================
    // Visual
    // =========================
    [Header("Visual")]
    [SerializeField] private MarkerVisual visual;

    // =========================
    // State
    // =========================
    public bool IsSelected { get; private set; }

    // =========================
    // Properties
    // =========================
    public string DefinitionId => definitionId;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        // 🔴 핵심: 프리팹이므로 런타임에 직접 찾는다
        visual = GetComponent<MarkerVisual>();
        if (visual == null)
            visual = GetComponentInChildren<MarkerVisual>();

        if (visual == null)
            visual = FindAnyObjectByType<MarkerVisual>();

        if (visual == null)
            Debug.LogError("[MarkerInstance] MarkerVisual not found.");
    }
    
    
    // =========================
    // Initialization
    // =========================

    /// <summary>
    /// Definition ID로 초기화.
    /// Definition의 현재 상태(placement)에 따라
    /// Transform과 비주얼을 동기화한다.
    /// </summary>
    public void Initialize(string definitionId)
    {
        this.definitionId = definitionId;

        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.GetById(definitionId);

        if (def == null)
        {
            Debug.LogError($"[MarkerInstance] Definition not found : {definitionId}");
            return;
        }

        // Debug.Log($"[MarkerInstance.Initialize] color : {def.Color}");
        
        // 색상 적용
        visual.SetBaseColor(def.Color);

        // 배치된 상태라면 위치 동기화
        if (def.IsPlaced)
        {
            ApplyPlacement(def.Placement);
            visual.SetNormal();
        }
        else
        {
            // 미배치 상태: 기본 표현
            visual.SetUnplaced();
        }
    }

    // =========================
    // Placement Sync
    // =========================

    /// <summary>
    /// Definition의 확정된 배치 결과를
    /// Transform에 반영한다.
    /// </summary>
    public void ApplyPlacement(MarkerPlacement placement)
    {
        if (placement == null)
            return;

        transform.SetPositionAndRotation(
            placement.position,
            placement.rotation
        );
    }

    // =========================
    // Selection
    // =========================

    public void Select()
    {
        if (IsSelected)
            return;

        Debug.Log("[Marker Instance.Select] : ");
        IsSelected = true;
        visual.SetSelected();
    }

    public void Deselect()
    {
        if (!IsSelected)
            return;

        IsSelected = false;
        visual.SetNormal();
    }

    // =========================
    // Preview (Move Controller Only)
    // =========================

    /// <summary>
    /// 프리뷰 위치 반영 (데이터에는 기록하지 않음)
    /// </summary>
    public void SetPreviewPose(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
    }

    public void SetPreviewValid()
    {
        visual.SetPreviewValid();
    }

    public void SetPreviewInvalid()
    {
        visual.SetPreviewInvalid();
    }

    // =========================
    // Repository Sync (확정 시점 전용)
    // =========================

    /// <summary>
    /// 현재 Transform을 Repository에 기록한다.
    /// 반드시 "입력 컨트롤러가 성공 판단을 한 후"에만 호출되어야 한다.
    /// </summary>
    public void CommitPlacement()
    {
        MarkerDefinitionRepository.Instance.SetPlacement(
            definitionId,
            transform.position,
            transform.rotation
        );
        visual.SetNormal(); // ⭐ 반드시 복구
    }

    /// <summary>
    /// 배치 해제 (Repository 기준)
    /// </summary>
    public void ClearPlacement()
    {
        MarkerDefinitionRepository.Instance.ClearPlacement(definitionId);
    }
}
