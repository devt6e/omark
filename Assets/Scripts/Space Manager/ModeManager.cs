using UnityEngine;

public enum BuildMode
{
    None,
    MoveView,
    PlaceFurniture,
    DrawWall,
    DrawFloor
}

public class ModeManager : MonoBehaviour
{
    public static ModeManager Instance;
    public BuildMode currentMode = BuildMode.None;

    void Awake()
    {
        Instance = this;
    }

    public void SetMode(BuildMode mode)
    {
        currentMode = mode;
        UIManager.Instance.UpdateModeTitle(mode);
    }
}
