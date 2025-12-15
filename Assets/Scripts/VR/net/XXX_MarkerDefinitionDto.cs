// using System;
// using UnityEngine;

// [Serializable]
// public class MarkerDefinitionDto
// {
//     // 요구사항의 "아이디" 역할 (프로젝트 내부 규칙: definitionId)
//     public string id;

//     public string name;
//     public string description;

//     public int colorIndex;
//     public ColorDto color;

//     public bool isFavorite;

//     // null 가능
//     public MarkerPlacementDto placement;
// }

// [Serializable]
// public class MarkerPlacementDto
// {
//     public Vector3 position;
//     public Quaternion rotation;
// }

// [Serializable]
// public class ColorDto
// {
//     public float r;
//     public float g;
//     public float b;
//     public float a;

//     public ColorDto() { }

//     public ColorDto(Color c)
//     {
//         r = c.r; g = c.g; b = c.b; a = c.a;
//     }

//     public Color ToColor() => new Color(r, g, b, a);
// }
