using UnityEngine;

public enum EditMode
{
    MoveView,
    DrawFloor,
    PlaceFurniture    
}

public class EditorModeManager : MonoBehaviour
{
    public static EditorModeManager Instance;

    public EditMode CurrentMode { get; private set; } = EditMode.MoveView;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMode(EditMode newMode)
    {
        CurrentMode = newMode;
        Debug.Log("Mode changed to: " + newMode);
    }
}
