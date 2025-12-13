using UnityEngine;
using System;

[Serializable]
public class T6MarkerData
{
    [Header("Identity")]
    public string markerId;          // 고유 ID
    public string displayName;       // UI 표시용 이름
    public string description;       // 설명
    public Color color;              // 마커 기본 색상

    [Header("Placement")]
    public bool isPlaced;            // 배치 상태
    public Vector3 position;         // 월드 좌표
    public Quaternion rotation;      // 회전

    public T6MarkerData(string name, Color color)
    {
        markerId = Guid.NewGuid().ToString();
        displayName = name;
        this.color = color;
        position = new Vector3(0f,0.1f,0f);
        rotation = Quaternion.identity;
    }

    public void UpdateTransform(Transform t)
    {
        position = t.position;
        rotation = t.rotation;
    }
}
