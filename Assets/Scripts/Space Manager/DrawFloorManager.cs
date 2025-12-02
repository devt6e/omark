using UnityEngine;

public class DrawFloorManager : MonoBehaviour
{
    public static DrawFloorManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void BeginWallDraw()
    {
        Debug.Log("Wall draw start");
    }

    public void BeginFloorDraw()
    {
        Debug.Log("Floor draw start");
    }
}
