using UnityEngine;
using System;

public class FloorPiece : MonoBehaviour
{
    // ================================
    // 저장용 ID
    // ================================
    public string pieceId;

    // ================================
    // 하이라이트 관련
    // ================================
    private GameObject highlightObj;

    [Header("Highlight Prefab (Quad)")]
    public GameObject highlightPrefab;


    private void Awake()
    {
        // JSON 로드 시 기존 ID 유지됨
        if (string.IsNullOrEmpty(pieceId))
            pieceId = Guid.NewGuid().ToString();
    }

    // ================================
    // 데이터 → JSON 변환
    // ================================
    public FloorPieceData ToData()
    {
        return new FloorPieceData
        {
            id = pieceId,
            position = transform.localPosition,
            scale = transform.localScale
        };
    }

    // ================================
    // JSON → 데이터 적용
    // ================================
    public void FromData(FloorPieceData data)
    {
        pieceId = data.id;
        transform.localPosition = data.position;
        transform.localScale = data.scale;
    }

    // ================================
    // 선택 (하이라이트 표시)
    // ================================
    public void Select()
    {
        if (highlightObj != null) return;

        highlightObj = Instantiate(highlightPrefab, transform);

        highlightObj.transform.localPosition = new Vector3(0, 0.51f, 0);
        highlightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        highlightObj.transform.localScale = Vector3.one;
    }

    // ================================
    // 선택 해제
    // ================================
    public void Deselect()
    {
        if (highlightObj != null)
        {
            Destroy(highlightObj);
            highlightObj = null;
        }
    }

    // ================================
    // 바닥 Bounds 반환
    // ================================
    public Bounds GetBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }
}