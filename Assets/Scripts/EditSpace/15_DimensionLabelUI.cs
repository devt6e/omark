using UnityEngine;
using TMPro;

public class DimensionLabelUI : MonoBehaviour
{
    public TextMeshProUGUI label;

    // 화면 좌표로 UI 배치
    public void SetLabel(Vector2 screenPos, float lengthValue)
    {
        transform.position = screenPos;
        label.text = $"{lengthValue * 100f:F0}cm"; // cm 단위 표시
    }
}
