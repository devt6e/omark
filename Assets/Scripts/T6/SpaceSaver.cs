using System.Collections.Generic;
using UnityEngine;

public class T6SpaceSaver : MonoBehaviour
{
    public T6SpaceDetail BuildDetail(string spaceName)
    {
        T6SpaceDetail detail = new T6SpaceDetail();
        detail.meta.name = spaceName;

        // Floors
        var floors = FindObjectsByType<FloorPiece>(FindObjectsSortMode.None);
        foreach (var f in floors)
            detail.floors.Add(f.ToT6Data());

        // Furnitures
        var furnitures = FindObjectsByType<FurniturePiece>(FindObjectsSortMode.None);
        foreach (var fu in furnitures)
        {
            detail.furnitures.Add(new T6FurnitureData
            {
                id = fu.Id,
                position = fu.transform.localPosition,
                rotation = fu.transform.localRotation,
                size = fu.Size
            });
        }

        return detail;
    }

    public string BuildJson(string spaceName)
    {
        return T6SpaceDetailSerializer.ToJson(BuildDetail(spaceName));
    }
}
