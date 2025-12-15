using UnityEngine;

/// <summary>
/// MarkerInstance의 시각적 표현 전용 컴포넌트.
/// - 데이터/입력/판단 로직 없음
/// - 상태에 따른 표현만 담당
/// </summary>
public class MarkerVisual : MonoBehaviour
{
    // =========================
    // Renderer
    // =========================
    [Header("Renderers")]
    [SerializeField] private Renderer[] renderers;

    // =========================
    // Scale / Offset
    // =========================
    [Header("Scale")]
    [SerializeField] private float selectedScaleMultiplier = 1.2f;

    [Header("YOffset")]
    [SerializeField] private float selectedYOffset = 0.15f;

    // =========================
    // Alpha
    // =========================
    [Header("Alpha")]
    [SerializeField] private float normalAlpha = 1f;
    [SerializeField] private float selectedAlpha = 0.85f;
    [SerializeField] private float previewAlpha = 0.35f;

    // =========================
    // Internal State
    // =========================
    private Color baseColor = Color.white;
    private Vector3 originalScale;
    private Vector3 originalLocalPos;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        originalScale = transform.localScale;
        originalLocalPos = transform.localPosition;
    }

    // =========================
    // Base Setup
    // =========================

    /// <summary>
    /// MarkerDefinition의 색상을 기반으로 기본 색상 설정
    /// </summary>
    public void SetBaseColor(Color color)
    {
        baseColor = color;
        // Debug.Log($"[MarkerVisual.SetBaseColor] color : {color}");
        ApplyColor(baseColor, normalAlpha);
    }

    // =========================
    // States
    // =========================

    /// <summary>
    /// 기본 상태 (배치됨 / 선택 안 됨)
    /// </summary>
    public void SetNormal()
    {
        transform.localScale = originalScale;
        if(transform.localPosition.y != 0.1f)
            transform.localPosition = new Vector3(transform.localPosition.x, 0.15f, transform.localPosition.z);
        originalLocalPos = transform.localPosition;
        ApplyColor(baseColor, normalAlpha);
    }

    /// <summary>
    /// 선택된 상태
    /// </summary>
    public void SetSelected()
    {
        Debug.Log("[MarkerVisual.SetSelected] : ");
        transform.localScale = originalScale * selectedScaleMultiplier;
        transform.localPosition += Vector3.up * selectedYOffset;
        ApplyColor(baseColor, selectedAlpha);
    }

    /// <summary>
    /// 아직 배치되지 않은 상태 (인벤토리에서 막 나온 경우)
    /// </summary>
    public void SetUnplaced()
    {
        transform.localScale = originalScale;
        transform.localPosition = originalLocalPos;
        ApplyColor(baseColor, previewAlpha);
    }

    /// <summary>
    /// 프리뷰 상태 - 배치 가능
    /// </summary>
    public void SetPreviewValid()
    {
        ApplyColor(baseColor, previewAlpha);
    }

    /// <summary>
    /// 프리뷰 상태 - 배치 불가
    /// </summary>
    public void SetPreviewInvalid()
    {
        ApplyColor(Color.red, previewAlpha);
    }

    // =========================
    // Internal Utility
    // =========================
    private void ApplyColor(Color color, float alpha)
    {
        color.a = alpha;

        foreach (var r in renderers)
        {
            if (r == null) continue;

            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = color;
            }
        }
    }
}
