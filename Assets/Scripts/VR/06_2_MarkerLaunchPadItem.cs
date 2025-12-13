using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MarkerLaunchPadItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private Image colorImage;
    [SerializeField] private GameObject placedIndicator; // 배치됨 표시용 (선택)

    private T6MarkerItemData data;

    public void Bind(T6MarkerItemData markerData)
    {
        data = markerData;

        txtName.text = data.name;
        colorImage.color = data.color;

        RefreshState();
    }

    public T6MarkerItemData GetData()
    {
        return data;
    }

    public void UpdatePlacement(Vector3 pos, Quaternion rot)
    {
        data.UpdatePlacement(pos, rot);
        RefreshState();
    }

    private void RefreshState()
    {
        if (placedIndicator != null)
            placedIndicator.SetActive(data.isPlaced);
    }
}
