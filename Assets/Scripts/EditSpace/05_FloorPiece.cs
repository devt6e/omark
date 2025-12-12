using UnityEngine;
using System;

public class FloorPiece : MonoBehaviour
{
    public static FloorPiece Instance {get; private set;}
    // 저장용 ID
    public string pieceId;

    // 하이라이트 관련
    private GameObject highlightObj;

    private float width;
    private float depth;
    private Vector3 center;
    private Vector3 scale;

    [Header("Highlight Prefab (Quad)")]
    public GameObject highlightPrefab;


    private void Awake()
    {
        Instance = this;
        if (string.IsNullOrEmpty(pieceId))
            pieceId = Guid.NewGuid().ToString();
    }

    // ================================
    // JSON 변환 (T6 Prefix)
    // ================================
    public T6FloorData ToT6Data()
    {
        return new T6FloorData
        {
            id = pieceId,
            position = transform.localPosition,
            scale = transform.localScale
        };
    }

    public void FromT6Data(T6FloorData data)
    {
        pieceId = data.id;
        transform.localPosition = data.position;
        transform.localScale = data.scale;
    }

    // ================================
    // 선택 시 하이라이트 ON
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
    // 선택 해제 시 하이라이트 OFF
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
    // Bounds 계산
    // ================================
    public Bounds GetBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }
}
