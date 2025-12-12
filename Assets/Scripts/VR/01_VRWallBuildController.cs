using UnityEngine;

public class VRWallBuildController : MonoBehaviour
{
    private void Start()
    {
        var wallGen = FindFirstObjectByType<WallGenerator>();
        if (wallGen == null)
        {
            Debug.LogWarning("VRWallBuildController: WallGenerator 없음");
            return;
        }

        wallGen.RegenerateWalls();
    }
}
