using UnityEngine;
using System.Collections.Generic;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }

    private readonly List<FurniturePiece> pieces = new List<FurniturePiece>();
    public IReadOnlyList<FurniturePiece> Pieces => pieces;

    private FurniturePiece currentSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 가구 생성 시 자동 등록
    /// </summary>
    public void Register(FurniturePiece piece)
    {
        if (piece != null && !pieces.Contains(piece))
            pieces.Add(piece);
    }

    /// <summary>
    /// 가구 삭제 시 호출
    /// </summary>
    public void Unregister(FurniturePiece piece)
    {
        if (piece == null) return;

        if (pieces.Contains(piece))
            pieces.Remove(piece);

        if (currentSelected == piece)
            currentSelected = null;
    }

    /// <summary>
    /// 현재 선택된 가구 반환
    /// </summary>
    public FurniturePiece GetSelected()
    {
        return currentSelected;
    }

    /// <summary>
    /// 가구 선택 처리
    /// </summary>
    public void Select(FurniturePiece piece)
    {
        if (currentSelected == piece)
            return;

        // 이전 선택 해제
        if (currentSelected != null)
            currentSelected.Deselect();

        currentSelected = piece;

        if (currentSelected != null)
            currentSelected.Select();
    }

    /// <summary>
    /// 선택 해제
    /// </summary>
    public void ClearSelection()
    {
        if (currentSelected != null)
            currentSelected.Deselect();

        currentSelected = null;
    }

    /// <summary>
    /// 전체 가구 목록 반환
    /// </summary>
    public IEnumerable<FurniturePiece> GetAll()
    {
        return pieces;
    }
}
