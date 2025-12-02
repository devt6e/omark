using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public Camera mainCam;
    public bool is3DView = true;

    void Awake()
    {
        Instance = this;
    }

    public void ToggleViewMode()
    {
        is3DView = !is3DView;
        UIManager.Instance.UpdateViewModeIcon(is3DView);
    }
}
