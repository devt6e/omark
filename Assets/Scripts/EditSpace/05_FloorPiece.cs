using UnityEngine;
using System;

public class FloorPiece : MonoBehaviour
{
    // ================================
    // Size UI (싱글턴으로 연결됨)
    // ================================
    // public SizeUIController sizeUI;   // 선택 시 자동 연결됨

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
        if (string.IsNullOrEmpty(pieceId))
            pieceId = Guid.NewGuid().ToString();

        // // 싱글턴 연결
        // if (sizeUI == null)
        //     sizeUI = SizeUIController.Instance;

        // if (sizeUI != null)
        //     sizeUI.Hide();
    }

    // ================================
    // JSON 변환
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

    public void FromData(FloorPieceData data)
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

        // // UI도 숨김
        // HideSizeUI();
    }

    // ================================
    // Bounds 계산
    // ================================
    public Bounds GetBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }

    // ================================
    // UI 출력 / 숨기기
    // ================================
    // public void ShowSizeUI()
    // {
    //     if (sizeUI == null) return;

    //     sizeUI.Show(this);
    //     sizeUI.UpdateUIPositions(this);
    // }

    // public void HideSizeUI()
    // {
    //     if (sizeUI != null)
    //         sizeUI.Hide();
    // }

    // // ================================
    // // 실제 바닥 크기 적용 메서드
    // // ================================
    // public void ApplyWidth(float newWidth)
    // {
    //     Vector3 scale = transform.localScale;
    //     scale.x = Mathf.Max(0.01f, newWidth); // 최소 크기 제한
    //     transform.localScale = scale;

    //     sizeUI?.UpdateUIPositions(this);
    // }

    // public void ApplyHeight(float newHeight)
    // {
    //     Vector3 scale = transform.localScale;
    //     scale.z = Mathf.Max(0.01f, newHeight);
    //     transform.localScale = scale;

    //     sizeUI?.UpdateUIPositions(this);
    // }
}
