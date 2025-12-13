using UnityEngine;
using System;

[Serializable]
public class T6MarkerItemData
{
    public string id;

    // 정의 정보
    public string name;
    public string description;
    public Color color;

    // 배치 상태
    public bool isPlaced;
    public Vector3 placedPosition;
    public Quaternion placedRotation;

    public T6MarkerItemData(string name, string description, Color color)
    {
        id = Guid.NewGuid().ToString();
        this.name = name;
        this.description = description;
        this.color = color;

        isPlaced = false;
        placedPosition = Vector3.zero;
        placedRotation = Quaternion.identity;
    }

    public void UpdatePlacement(Vector3 position, Quaternion rotation)
    {
        isPlaced = true;
        placedPosition = position;
        placedRotation = rotation;
    }
}
