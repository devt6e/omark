using UnityEngine;
using System.Collections.Generic;

public static class SnapUtil
{
    // ================================
    // 설정 값
    // ================================
    public static float GridUnit = 0.1f;         // 10cm
    public static float SnapThreshold = 0.15f;   // 피스끼리 붙는 거리 허용 범위

    // ================================
    // FloorPiece 이동 시 호출되는 함수
    // ================================
    public static Vector3 SnapFloorPiecePosition(Vector3 rawPos, FloorPiece movingPiece, float threshold)
    {
        SnapThreshold = threshold;

        // 1) 먼저 FloorPiece 스냅 시도
        Vector3? snapPos = TrySnapToNeighbors(rawPos, movingPiece);

        if (snapPos.HasValue)
            return snapPos.Value;

        // 2) 스냅 실패 → 그리드 스냅
        return SnapToGrid(rawPos);
    }

    // ============================================================
    // 1) 바닥 피스끼리 스냅
    // ============================================================
    private static Vector3? TrySnapToNeighbors(Vector3 rawPos, FloorPiece movingPiece)
    {
        List<FloorPiece> all = RoomManager.Instance.GetAllPieces();

        Bounds movingBounds = movingPiece.GetBounds();
        Vector3 size = movingBounds.size;

        float halfW = size.x * 0.5f;
        float halfD = size.z * 0.5f;

        // 이동 후의 임시 Bounds
        Bounds future = new Bounds(rawPos, size);

        foreach (var other in all)
        {
            if (other == null || other == movingPiece) continue;

            Bounds o = other.GetBounds();

            // -----------------------------------------
            // LEFT 스냅 (moving.right ≈ other.left)
            // -----------------------------------------
            float movingRight = future.center.x + halfW;
            float otherLeft   = o.min.x;

            if (Mathf.Abs(movingRight - otherLeft) <= SnapThreshold)
            {
                if (CheckZOverlap(future, o))
                {
                    float correctedX = otherLeft - halfW;
                    return new Vector3(correctedX, rawPos.y, rawPos.z);
                }
            }

            // -----------------------------------------
            // RIGHT 스냅 (moving.left ≈ other.right)
            // -----------------------------------------
            float movingLeft = future.center.x - halfW;
            float otherRight = o.max.x;

            if (Mathf.Abs(movingLeft - otherRight) <= SnapThreshold)
            {
                if (CheckZOverlap(future, o))
                {
                    float correctedX = otherRight + halfW;
                    return new Vector3(correctedX, rawPos.y, rawPos.z);
                }
            }

            // -----------------------------------------
            // TOP 스냅 (moving.bottom ≈ other.top)
            // -----------------------------------------
            float movingBottomZ = future.center.z - halfD;
            float otherTopZ     = o.max.z;

            if (Mathf.Abs(movingBottomZ - otherTopZ) <= SnapThreshold)
            {
                if (CheckXOverlap(future, o))
                {
                    float correctedZ = otherTopZ + halfD;
                    return new Vector3(rawPos.x, rawPos.y, correctedZ);
                }
            }

            // -----------------------------------------
            // BOTTOM 스냅 (moving.top ≈ other.bottom)
            // -----------------------------------------
            float movingTopZ = future.center.z + halfD;
            float otherBottomZ = o.min.z;

            if (Mathf.Abs(movingTopZ - otherBottomZ) <= SnapThreshold)
            {
                if (CheckXOverlap(future, o))
                {
                    float correctedZ = otherBottomZ - halfD;
                    return new Vector3(rawPos.x, rawPos.y, correctedZ);
                }
            }
        }

        return null; // 스냅 실패
    }

    // ============================================================
    // 2) 그리드 스냅
    // ============================================================
    public static Vector3 SnapToGrid(Vector3 pos)
    {
        pos.x = Mathf.Round(pos.x / GridUnit) * GridUnit;
        pos.z = Mathf.Round(pos.z / GridUnit) * GridUnit;
        return pos;
    }

    // ============================================================
    // Overlap 체크 함수 (XZ)
    // ============================================================
    private static bool CheckXOverlap(Bounds a, Bounds b)
    {
        return (a.max.x > b.min.x) && (a.min.x < b.max.x);
    }

    private static bool CheckZOverlap(Bounds a, Bounds b)
    {
        return (a.max.z > b.min.z) && (a.min.z < b.max.z);
    }
}
