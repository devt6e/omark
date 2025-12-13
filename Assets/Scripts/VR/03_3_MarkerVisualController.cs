using UnityEngine;

public class MarkerVisualController : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;

    [Header("Scale")]
    [SerializeField] private float selectedScaleMultiplier = 1.2f;

    [Header("YOffset")]
    [SerializeField] private float selectedYOffset = 0.1f;

    [Header("Alpha")]
    [SerializeField] private float normalAlpha = 1f;
    [SerializeField] private float selectedAlpha = 0.8f;
    [SerializeField] private float previewAlpha = 0.35f;

    private Color baseColor = Color.white;
    private Vector3 originalScale;
    private Vector3 originalLocalPos;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalLocalPos = transform.localPosition;
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;
        ApplyColor(color, normalAlpha);
    }

    public void SetNormal()
    {
        transform.localScale = originalScale;
        originalLocalPos = transform.localPosition;
        ApplyColor(baseColor, normalAlpha);
    }

    public void SetSelected()
    {
        transform.localScale = originalScale * selectedScaleMultiplier;
        transform.localPosition += Vector3.up * selectedYOffset;
        ApplyColor(baseColor, selectedAlpha);
    }

    public void SetSelected(bool isNew)
    {
        if(isNew)
            transform.localPosition = transform.localPosition;
        transform.localScale = originalScale * selectedScaleMultiplier;
        
        ApplyColor(baseColor, selectedAlpha);
    }

    public void SetPreviewValid()
    {
        ApplyColor(Color.green, previewAlpha);
    }

    public void SetPreviewInvalid()
    {
        ApplyColor(Color.red, previewAlpha);
    }

    private void ApplyColor(Color c, float alpha)
    {
        c.a = alpha;

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                mat.color = c;
            }
        }
    }
}
