using System.Collections.Generic;

public static class SelectionManagerAccessor
{
    public static List<FloorPiece> GetSelection()
    {
        return SelectionManager.Instance?.GetCurrentSelection();
    }
}