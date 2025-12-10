using UnityEngine;

public class DimensionLabelUIManager : MonoBehaviour
{
    public static DimensionLabelUIManager Instance;

    [Header("Preview Labels (FloorDrawer)")]
    public DimensionLabelUI widthPreviewLabel;
    public DimensionLabelUI heightPreviewLabel;

    [Header("Actual Labels (SelectionManager)")]
    public DimensionLabelUI widthActualLabel;
    public DimensionLabelUI heightActualLabel;

    private void Awake()
    {
        Instance = this;
    }

    public void HidePreview()
    {
        widthPreviewLabel.Hide();
        heightPreviewLabel.Hide();
    }

    public void HideActual()
    {
        widthActualLabel.Hide();
        heightActualLabel.Hide();
    }
}
