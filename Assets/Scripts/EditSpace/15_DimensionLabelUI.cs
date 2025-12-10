using UnityEngine;
using TMPro;

public class DimensionLabelUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI label;

    [Header("Placement Settings")]
    public Vector3 worldOffset = new Vector3(0, 0.15f, 0); // 기본 오프셋
    public bool placeAbove = true;     // 위쪽 배치 (가로 라벨)
    public bool placeRight = false;    // 오른쪽 배치 (세로 라벨)

    [Header("Scaling")]
    public float minScale = 0.6f;
    public float maxScale = 1.6f;
    public float scaleDistanceFactor = 12f;

    private Camera cam;
    private Vector3 worldTarget;

    private void Awake()
    {
        cam = Camera.main;
        // gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!gameObject.activeSelf) return;

        UpdatePosition();
        UpdateScale();
    }

    // ============================================================
    // 외부에서 호출하는 메인 API
    // ============================================================
    public void SetWorldLabel(Vector3 worldPos, float lengthValue)
    {
        worldTarget = worldPos + worldOffset;
        label.text = $"{lengthValue * 100f:F0} cm";

        gameObject.SetActive(true);

        UpdatePosition();
        UpdateScale();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // ============================================================
    // 치수 라벨 위치 계산 (위쪽/오른쪽 자동 배치 지원)
    // ============================================================
    private void UpdatePosition()
    {
        if (cam == null) cam = Camera.main;

        // 화면 공간 좌표 변환
        Vector2 screenPos = cam.WorldToScreenPoint(worldTarget);

        // 자동 배치 (위 / 오른쪽)
        Vector2 offset = Vector2.zero;

        if (placeAbove)
            offset += new Vector2(0, 25f);  // 위쪽 여백

        if (placeRight)
            offset += new Vector2(40f, 0);  // 오른쪽 여백

        transform.position = screenPos + offset;
    }

    // ============================================================
    // 카메라 줌에 따른 자동 스케일
    // ============================================================
    private void UpdateScale()
    {
        if (cam == null) cam = Camera.main;

        float dist = cam.orthographic
            ? cam.orthographicSize
            : Vector3.Distance(cam.transform.position, worldTarget);

        float t = Mathf.Clamp(dist / scaleDistanceFactor, 0f, 1f);
        float scale = Mathf.Lerp(minScale, maxScale, t);

        transform.localScale = Vector3.one * scale;
    }
}
