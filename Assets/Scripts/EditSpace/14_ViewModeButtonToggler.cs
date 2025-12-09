using UnityEngine;

public class ViewModeButtonToggler : MonoBehaviour
{
    [Header("UI Buttons")]
    public GameObject drawFloorButton;     // 2D 모드에서만 보임
    public GameObject furnitureButton;     // 3D 모드에서만 보임

    // ViewModeController가 is3D 상태를 넘겨서 호출함
    public void ApplyViewMode(bool is3DView)
    {
        if (drawFloorButton != null)
            drawFloorButton.SetActive(!is3DView);   // 2D 전용

        if (furnitureButton != null)
            furnitureButton.SetActive(is3DView);    // 3D 전용
    }
}
