using System;
using UnityEngine;

#region Placement

/// <summary>
/// 마커의 "확정된 배치 결과"만을 담는 값 객체
/// 프리뷰/이동 중 상태는 절대 여기에 기록되지 않는다.
/// </summary>
[Serializable]
public class MarkerPlacement
{
    public Vector3 position;
    public Quaternion rotation;

    public MarkerPlacement(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

#endregion


/// <summary>
/// 마커의 논리적 실체(정의).
/// 공간에 배치되었는지 여부와 무관하게 항상 존재한다.
/// </summary>
[Serializable]
public class MarkerDefinition
{
    // =========================
    // Identity
    // =========================
    [Header("Identity")]
    [SerializeField] private string definitionId;

    // =========================
    // Display / UI
    // =========================
    [Header("Display")]
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private Color color;
    [SerializeField] private int colorIndex;
    [SerializeField] public bool isFavorite { get; private set; } = false;


    // =========================
    // Placement (nullable)
    // =========================
    [Header("Placement (nullable)")]
    [SerializeField] private MarkerPlacement placement;

    // =========================
    // Properties (Read Only)
    // =========================
    public string DefinitionId => definitionId;
    public string DisplayName => displayName;
    public string Description => description;
    public Color Color => color;
    public int ColorIndex => colorIndex;
    public bool IsFavorite => isFavorite;

    /// <summary>
    /// 현재 배치되어 있는지 여부
    /// </summary>
    public bool IsPlaced => placement != null;

    /// <summary>
    /// 확정된 배치 정보 (없으면 null)
    /// </summary>
    public MarkerPlacement Placement => placement;

    // =========================
    // Constructor
    // =========================
    public MarkerDefinition(string displayName, Color color, int colorIndex, string description = "")
    {
        this.definitionId = Guid.NewGuid().ToString();
        this.displayName = displayName;
        this.color = color;
        this.colorIndex = colorIndex;
        this.description = description;
        this.placement = null;
    }

    // =========================
    // Placement Control
    // =========================

    /// <summary>
    /// 배치 확정 (결과 기록)
    /// 판단 로직은 외부(입력 컨트롤러)에 있다.
    /// </summary>
    public void SetPlacement(Vector3 position, Quaternion rotation)
    {
        placement = new MarkerPlacement(position, rotation);
    }

    /// <summary>
    /// 배치 해제 (미배치 상태로 전환)
    /// </summary>
    public void ClearPlacement()
    {
        placement = null;
    }
    public void UpdateInfo(string displayName, string description, int colorIndex, Color color)
    {
        this.displayName = displayName;
        this.description = description;
        this.colorIndex = colorIndex;
        this.color = color;
    }
    public void SetFavorite(bool value)
    {
        this.isFavorite = value;
    }
}
