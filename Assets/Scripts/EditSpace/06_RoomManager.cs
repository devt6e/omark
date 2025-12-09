using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    // 현재 공간에 존재하는 모든 바닥 조각들
    private List<FloorPiece> floorPieces = new List<FloorPiece>();

    // 인접 판정 허용 오차 (그리드 스냅 기준 조금의 오차 허용)
    // [SerializeField] private float touchTolerance = 0.001f;

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

    // 새로 만들려고 하는 바닥 Bounds가 "생성 가능한 위치"인지 확인
    public bool CanPlace(Bounds candidate)
    {
        //-----항상 혀용-----
        return true;
        //------------------

        // // 아직 아무 바닥도 없으면: 첫 바닥은 항상 허용
        // if (floorPieces.Count == 0)
        //     return true;

        // // 기존 모든 FloorPiece와 비교
        // foreach (var piece in floorPieces)
        // {
        //     if (piece == null) continue;

        //     Bounds b = piece.GetBounds();

        //     if (IsAdjacentOrOverlapping2D(b, candidate))
        //         return true;
        // }

        // // 어느 것도 안 닿으면 생성 불가
        // return false;
    }

    // 2D(XZ 평면) 기준으로 "겹치거나" / "변이 닿는지" 판정
    // private bool IsAdjacentOrOverlapping2D(Bounds a, Bounds b)
    // {
    //     // XZ 평면에서의 min/max
    //     float aMinX = a.min.x;
    //     float aMaxX = a.max.x;
    //     float aMinZ = a.min.z;
    //     float aMaxZ = a.max.z;

    //     float bMinX = b.min.x;
    //     float bMaxX = b.max.x;
    //     float bMinZ = b.min.z;
    //     float bMaxZ = b.max.z;

    //     // 1) 우선 "겹치는" 경우 (overlap) → 무조건 OK
    //     bool overlapX = aMaxX > bMinX + touchTolerance && aMinX < bMaxX - touchTolerance;
    //     bool overlapZ = aMaxZ > bMinZ + touchTolerance && aMinZ < bMaxZ - touchTolerance;

    //     if (overlapX && overlapZ)
    //         return false;

    //     // 2) 변이 "닿는" 경우 (edge-touch)

    //     // 수평 방향 변 닿기 (x 방향으로 닿고, z 범위가 겹쳐야 함)
    //     bool touchRight =
    //         Mathf.Abs(aMaxX - bMinX) <= touchTolerance &&    // a 오른쪽 == b 왼쪽
    //         aMaxZ > bMinZ + touchTolerance && aMinZ < bMaxZ - touchTolerance;

    //     bool touchLeft =
    //         Mathf.Abs(aMinX - bMaxX) <= touchTolerance &&    // a 왼쪽 == b 오른쪽
    //         aMaxZ > bMinZ + touchTolerance && aMinZ < bMaxZ - touchTolerance;

    //     // 수직 방향 변 닿기 (z 방향으로 닿고, x 범위가 겹쳐야 함)
    //     bool touchTop =
    //         Mathf.Abs(aMaxZ - bMinZ) <= touchTolerance &&    // a 위 == b 아래
    //         aMaxX > bMinX + touchTolerance && aMinX < bMaxX - touchTolerance;

    //     bool touchBottom =
    //         Mathf.Abs(aMinZ - bMaxZ) <= touchTolerance &&    // a 아래 == b 위
    //         aMaxX > bMinX + touchTolerance && aMinX < bMaxX - touchTolerance;

    //     return touchRight || touchLeft || touchTop || touchBottom;
    // }

    public List<FloorPiece> GetAllPieces()
    {
        return floorPieces;
    }

    // ===========================================================
    // 현재 FloorPiece가 “중간 바닥”인지 판정 (삭제 불가 조건)
    // ===========================================================
    // public bool IsMiddlePiece(FloorPiece target)
    // {
    //     Bounds t = target.GetBounds();

    //     int connectedCount = 0;

    //     foreach (var other in floorPieces)
    //     {
    //         if (other == target) continue;

    //         Bounds o = other.GetBounds();

    //         // LEFT: target.xMin == other.xMax
    //         bool leftTouch =
    //             Mathf.Abs(t.min.x - o.max.x) <= touchTolerance &&
    //             t.max.z > o.min.z + touchTolerance &&
    //             t.min.z < o.max.z - touchTolerance;

    //         // RIGHT
    //         bool rightTouch =
    //             Mathf.Abs(t.max.x - o.min.x) <= touchTolerance &&
    //             t.max.z > o.min.z + touchTolerance &&
    //             t.min.z < o.max.z - touchTolerance;

    //         // TOP (Z+)
    //         bool topTouch =
    //             Mathf.Abs(t.max.z - o.min.z) <= touchTolerance &&
    //             t.max.x > o.min.x + touchTolerance &&
    //             t.min.x < o.max.x - touchTolerance;

    //         // BOTTOM (Z-)
    //         bool bottomTouch =
    //             Mathf.Abs(t.min.z - o.max.z) <= touchTolerance &&
    //             t.max.x > o.min.x + touchTolerance &&
    //             t.min.x < o.max.x - touchTolerance;

    //         if (leftTouch || rightTouch || topTouch || bottomTouch)
    //             connectedCount++;
    //     }

    //     // 2개 이상 붙어있는 경우 → 중간 FloorPiece (삭제 불가)
    //     return connectedCount >= 2;
    // }

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
