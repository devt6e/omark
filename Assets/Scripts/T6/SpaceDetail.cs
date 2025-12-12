using System;
using System.Collections.Generic;

[Serializable]
public class T6SpaceDetail
{
    public T6SpaceMetaData meta = new T6SpaceMetaData();

    public List<T6FloorData> floors = new List<T6FloorData>();
    public List<T6FurnitureData> furnitures = new List<T6FurnitureData>();

    // markers는 이후 추가
}