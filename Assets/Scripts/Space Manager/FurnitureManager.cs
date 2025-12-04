using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnFurniture(string furnitureId)
    {
        Debug.Log("Spawn Furniture: " + furnitureId);
    }
}

