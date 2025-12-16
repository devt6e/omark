using System;
using UnityEngine;

#region Placement

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
#region Custom
[Serializable]
public class MarkerCustomizing
{
    public string IconImagePath;                 // 저장용 (png, jpg)
    public string ModelGlbPath;                  // 저장용 (glb)

    public MarkerCustomizing(string IconImagePath, string ModelGlbPath)
    {
        this.IconImagePath = IconImagePath;
        this.ModelGlbPath = ModelGlbPath;
    }
}
#endregion

[Serializable]
public class MarkerDefinition
{
    [Header("Identity")]
    [SerializeField] private string definitionId;

    [Header("Display")]
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private Color color;
    [SerializeField] private int colorIndex;
    [SerializeField] public bool isFavorite { get; private set; } = false;

    [Header("Placement (nullable)")]
    [SerializeField] private MarkerPlacement placement;
    [SerializeField] private MarkerCustomizing customizing;

    public string DefinitionId => definitionId;
    public string DisplayName => displayName;
    public string Description => description;
    public Color Color => color;
    public int ColorIndex => colorIndex;
    public bool IsFavorite => isFavorite;

    public bool IsPlaced => placement != null;
    public MarkerPlacement Placement => placement;
    public bool IsCustomized => customizing != null;
    public MarkerCustomizing Customizing => customizing;

    // ✅ 신규 생성(클라이언트에서 생성) : GUID
    public MarkerDefinition(string displayName, Color color, int colorIndex, string description = "")
    {
        this.definitionId = Guid.NewGuid().ToString();
        this.displayName = displayName;
        this.color = color;
        this.colorIndex = colorIndex;
        this.description = description;
        this.placement = null;
        this.customizing = null;
    }

    // ✅ 로드(서버/파일에서 복원) : 고정 ID 사용
    public MarkerDefinition(string fixedId, string displayName, Color color, int colorIndex, string description, bool isFavorite, MarkerPlacement placement)
    {
        this.definitionId = fixedId;
        this.displayName = displayName;
        this.color = color;
        this.colorIndex = colorIndex;
        this.description = description;
        this.isFavorite = isFavorite;
        this.placement = placement;
        this.customizing = customizing;
    }

    public void SetPlacement(Vector3 position, Quaternion rotation)
    {
        placement = new MarkerPlacement(position, rotation);
    }

    public void ClearPlacement()
    {
        placement = null;
    }

    public void SetCustomizing(string iconImagePath, string modelGlbPath)
    {
        customizing = new MarkerCustomizing(iconImagePath, modelGlbPath);
    }

    public void ClearCustomizing()
    {
        customizing = null;
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
