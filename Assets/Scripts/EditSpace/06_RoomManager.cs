using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    // 현재 공간에 존재하는 모든 바닥 조각들
    [Header("Pieces(Runtime)")]
    [SerializeField] private List<FloorPiece> floorPieces = new List<FloorPiece>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 새 FloorPiece 등록
    public void RegisterPiece(FloorPiece piece)
    {
        if (!floorPieces.Contains(piece))
            floorPieces.Add(piece);
    }
    public void UnregisterPiece(FloorPiece piece)
    {
        if (floorPieces.Contains(piece))
            floorPieces.Remove(piece);
    }

    public bool CanPlace(Bounds candidate)
    {
        foreach (var piece in floorPieces)
        {
            if (piece == null) continue;

            Bounds other = piece.GetBounds();

            // XZ 평면에서 Overlap 체크
            bool overlapX = candidate.max.x > other.min.x && candidate.min.x < other.max.x;
            bool overlapZ = candidate.max.z > other.min.z && candidate.min.z < other.max.z;

            if (overlapX && overlapZ)
            {
                // 완전 겹침 → 배치 불가
                return false;
            }
        }

        return true;
    }


    public List<FloorPiece> GetAllPieces()
    {
        return floorPieces;
    }

    // ===========================================================
    // FloorPiece 삭제
    // ===========================================================
    public void DeletePiece(FloorPiece piece)
    {
        UnregisterPiece(piece);
        Destroy(piece.gameObject);
    }
    
    public Bounds GetRoomBounds()
    {
        if (floorPieces.Count == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds total = floorPieces[0].GetBounds();

        for (int i = 1; i < floorPieces.Count; i++)
        {
            total.Encapsulate(floorPieces[i].GetBounds());
        }

        return total;
    }
}
