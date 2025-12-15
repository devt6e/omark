using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SPACE.json 파일의 최상위 루트 DTO
/// - 공간 구조 전체를 복원하기 위한 저장 전용 데이터
/// - 서버 API DTO와 무관
/// </summary>
[Serializable]
public class SpaceSaveFileDto
{
    /// <summary>
    /// 파일 포맷 버전 (향후 마이그레이션 대비)
    /// </summary>
    public int version = 1;

    /// <summary>
    /// 공간 메타데이터
    /// </summary>
    public T6SpaceMetaData meta = new T6SpaceMetaData();

    /// <summary>
    /// 바닥 데이터 목록
    /// </summary>
    public List<T6FloorData> floors = new List<T6FloorData>();

    /// <summary>
    /// 가구 데이터 목록
    /// </summary>
    public List<T6FurnitureData> furnitures = new List<T6FurnitureData>();

    public List<SpaceMarkerDto> markers = new List<SpaceMarkerDto>();

    // ❌ markers는 포함하지 않음 (MARKER.json으로 분리)
}

#region Sub DTOs (기존 구조 재사용)

/// <summary>
/// 바닥 저장 데이터
/// </summary>
[Serializable]
public class T6FloorData
{
    public string id;

    /// <summary>
    /// 월드 기준 위치
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// 바닥 크기 (localScale 의미)
    /// </summary>
    public Vector3 scale;
}

/// <summary>
/// 가구 저장 데이터
/// </summary>
[Serializable]
public class T6FurnitureData
{
    public string id;

    /// <summary>
    /// 월드 기준 위치
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// 월드 기준 회전
    /// </summary>
    public Quaternion rotation;

    /// <summary>
    /// 가구 크기 (localScale 의미)
    /// </summary>
    public Vector3 size;
}

[Serializable]
public class SpaceMarkerDto
{
    public string id;
    public string name;
    public string description;

    public int colorIndex;
    public Color color;

    public bool isFavorite;

    // null 가능
    public SpaceMarkerPlacementDto placement;
}

[Serializable]
public class SpaceMarkerPlacementDto
{
    public Vector3 position;
    public Quaternion rotation;
}

/// <summary>
/// 공간 메타데이터
/// </summary>
[Serializable]
public class T6SpaceMetaData
{
    public string name;
    public string description;
}

#endregion
