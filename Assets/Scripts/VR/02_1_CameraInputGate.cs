// CameraInputGate.cs
public static class CameraInputGate
{
    public static bool IsLocked { get; private set; }

    public static bool CanProcessInput => !IsLocked;

    public static void Lock()
    {
        IsLocked = true;
    }

    public static void Unlock()
    {
        IsLocked = false;
    }
}
