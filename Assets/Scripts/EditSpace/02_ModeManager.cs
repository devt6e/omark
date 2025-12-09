using UnityEngine;

public enum EditMode
{
    // 2D View
    MoveView2D,
    DrawFloor,
    EditFloor,

    // 3D View
    MoveView3D,
    PlaceFurniture,
    EditFurniture
}

public class EditorModeManager : MonoBehaviour
{
    public static EditorModeManager Instance;

    public EditMode CurrentMode { get; private set; } = EditMode.MoveView2D;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMode(EditMode newMode)
    {
        CurrentMode = newMode;
        Debug.Log($"[ModeManager] Mode changed to: {newMode}");
    }

    public bool Is2DMode =>
        CurrentMode == EditMode.MoveView2D ||
        CurrentMode == EditMode.DrawFloor ||
        CurrentMode == EditMode.EditFloor;

    public bool Is3DMode =>
        CurrentMode == EditMode.MoveView3D ||
        CurrentMode == EditMode.PlaceFurniture ||
        CurrentMode == EditMode.EditFurniture;
}
