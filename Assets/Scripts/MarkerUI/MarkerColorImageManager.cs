using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

// Unity Inspector에서 색상 코드와 이미지를 묶어주는 구조체
[Serializable]
public struct MarkerColorSprite
{
    public string colorCode;    // 마커 데이터에 저장된 색상 코드 (예: "#FF0000")
    public Sprite markerSprite; // 해당 색상에 맞는 마커 이미지
}

public class MarkerColorImageManager : MonoBehaviour
{
    // Unity Inspector에 표시되어 이미지를 설정할 수 있는 리스트
    [Header("색상별 마커 이미지 설정")]
    public List<MarkerColorSprite> colorSprites;

    // 런타임 성능을 위한 Dictionary
    private Dictionary<string, Sprite> spriteLookup;

    // 이 스크립트의 인스턴스를 쉽게 찾을 수 있도록 Static 변수 사용 (선택 사항)
    public static MarkerColorImageManager Instance;

    void Awake()
    {
        // 싱글톤 패턴 (선택 사항)
        if (Instance == null)
        {
            Instance = this;
        }

        // 성능 향상을 위해 Dictionary로 변환
        spriteLookup = new Dictionary<string, Sprite>();
        foreach (var item in colorSprites)
        {
            if (!spriteLookup.ContainsKey(item.colorCode))
            {
                spriteLookup.Add(item.colorCode, item.markerSprite);
            }
        }
    }

    // 외부(UIMarkerItemData)에서 호출하여 Sprite를 가져가는 핵심 함수
    public Sprite GetSpriteByColorCode(string code)
    {
        // 코드에 혹시라도 공백이 포함될 경우를 대비해 Trim() 처리
        string key = code.Trim();

        if (spriteLookup.ContainsKey(key))
        {
            return spriteLookup[key];
        }

        // 기본값 (예: 디폴트 마커 이미지) 반환 및 경고
        Debug.LogWarning($"색상 코드 '{code}'에 해당하는 마커 이미지가 없습니다. 기본값을 사용합니다.");
        return null; // 또는 미리 정의된 기본 Sprite를 반환
    }
}