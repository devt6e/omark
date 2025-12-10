using UnityEngine;
using System.Collections.Generic;

public static class SnapUtil
{
    public static float GridUnit = 0.1f;        // 10cm
    public static float SnapThreshold = 0.15f;  // Edge Snap 거리
    public static float CornerThreshold = 0.15f; // Corner Snap 거리

    // ============================================================
    // Main API — SnapResult 반환 (Guide 표시용)
    // ============================================================
    public static SnapResult GetSnapResult(Vector3 rawPos, FloorPiece moving, float threshold)
    {
        SnapThreshold = threshold;

        // 1) Corner Snap (2개의 edge)
        SnapResult corner = TryCornerSnap(rawPos, moving);
        if (corner.snapped && CanMoveWithoutOverlap(corner.snappedPos, moving))
            return corner;

        // 2) Edge Snap (1개의 edge)
        SnapResult edge = TryEdgeSnap(rawPos, moving);
        if (edge.snapped && CanMoveWithoutOverlap(edge.snappedPos, moving))
            return edge;

        // 3) Grid Snap fallback
        Vector3 grid = SnapToGrid(rawPos);
        if (CanMoveWithoutOverlap(grid, moving))
        {
            return new SnapResult
            {
                snapped = false,
                snappedPos = grid,
                hasEdge = false,
                hasCorner = false
            };
        }

        // 4) rawPos 그대로
        return SnapResult.NoSnap(rawPos);
    }

    // ============================================================
    // SnapFloorPiecePosition(기존 호환 API)
    // ============================================================
    public static Vector3 SnapFloorPiecePosition(Vector3 rawPos, FloorPiece moving, float threshold)
    {
        return GetSnapResult(rawPos, moving, threshold).snappedPos;
    }

    // ============================================================
    // (A) Corner Snap — 교차되는 두 면을 계산하여 SnapResult 반환
    // ============================================================
    private static SnapResult TryCornerSnap(Vector3 rawPos, FloorPiece moving)
    {
        SnapResult r = SnapResult.NoSnap(rawPos);
        r.hasCorner = false;

        List<FloorPiece> all = RoomManager.Instance.GetAllPieces();

        Bounds mb = moving.GetBounds();
        float mHW = mb.size.x * 0.5f;
        float mHD = mb.size.z * 0.5f;

        Vector3 c = rawPos;

        // 이동 중 FloorPiece의 4개 코너
        Vector3[] mCorners = new Vector3[]
        {
            new Vector3(c.x - mHW, 0, c.z + mHD), // LT
            new Vector3(c.x - mHW, 0, c.z - mHD), // LB
            new Vector3(c.x + mHW, 0, c.z + mHD), // RT
            new Vector3(c.x + mHW, 0, c.z - mHD), // RB
        };

        foreach (var other in all)
        {
            if (other == null || other == moving) continue;

            Bounds ob = other.GetBounds();
            float oHW = ob.size.x * 0.5f;
            float oHD = ob.size.z * 0.5f;

            Vector3 oc = ob.center;

            // 기존 FloorPiece의 4개 코너
            Vector3[] oCorners = new Vector3[]
            {
                new Vector3(oc.x - oHW, 0, oc.z + oHD), // LT
                new Vector3(oc.x - oHW, 0, oc.z - oHD), // LB
                new Vector3(oc.x + oHW, 0, oc.z + oHD), // RT
                new Vector3(oc.x + oHW, 0, oc.z - oHD), // RB
            };

            // ===========================================
            // 코너끼리 거리 비교
            // ===========================================
            for (int i = 0; i < 4; i++)
            {
                Vector3 mc = mCorners[i];

                for (int j = 0; j < 4; j++)
                {
                    Vector3 ocn = oCorners[j];

                    float dist = Vector3.Distance(mc, ocn);
                    if (dist <= CornerThreshold)
                    {
                        // center 보정
                        Vector3 offset = rawPos - mc;
                        r.snappedPos = ocn + offset;

                        r.snapped = true;
                        r.hasCorner = true;
                        r.hasEdge = false;

                        // ======================================
                        // corner snap → 두 개의 edge 시각화
                        // ======================================

                        // edge A — 수직면
                        r.edgeStartA = new Vector3(oc.x - oHW, 0, ob.min.z);
                        r.edgeEndA   = new Vector3(oc.x - oHW, 0, ob.max.z);
                        
                        // edge B — 수평면
                        r.edgeStartB = new Vector3(ob.min.x, 0, oc.z - oHD);
                        r.edgeEndB   = new Vector3(ob.max.x, 0, oc.z - oHD);

                        return r;
                    }
                }
            }
        }

        return r;
    }

    // ============================================================
    // (B) Edge Snap — 1개의 면만 강조
    // ============================================================
    private static SnapResult TryEdgeSnap(Vector3 rawPos, FloorPiece moving)
    {
        SnapResult r = SnapResult.NoSnap(rawPos);
        r.hasEdge = false;

        List<FloorPiece> all = RoomManager.Instance.GetAllPieces();

        Bounds mb = moving.GetBounds();
        float mHW = mb.size.x * 0.5f;
        float mHD = mb.size.z * 0.5f;

        Vector3 c = rawPos;
        float mLeft = c.x - mHW;
        float mRight = c.x + mHW;
        float mBottom = c.z - mHD;
        float mTop = c.z + mHD;

        foreach (var other in all)
        {
            if (other == moving || other == null) continue;

            Bounds ob = other.GetBounds();
            float oLeft = ob.min.x;
            float oRight = ob.max.x;
            float oBottom = ob.min.z;
            float oTop = ob.max.z;

            // ---------------------------
            // RIGHT → LEFT
            // ---------------------------
            if (Mathf.Abs(mRight - oLeft) <= SnapThreshold &&
                Overlap(mBottom, mTop, oBottom, oTop))
            {
                r.snapped = true;
                r.hasEdge = true;
                r.snappedPos = new Vector3(oLeft - mHW, rawPos.y, rawPos.z);

                r.edgeStartA = new Vector3(oLeft, 0, oBottom);
                r.edgeEndA   = new Vector3(oLeft, 0, oTop);

                return r;
            }

            // ---------------------------
            // LEFT → RIGHT
            // ---------------------------
            if (Mathf.Abs(mLeft - oRight) <= SnapThreshold &&
                Overlap(mBottom, mTop, oBottom, oTop))
            {
                r.snapped = true;
                r.hasEdge = true;
                r.snappedPos = new Vector3(oRight + mHW, rawPos.y, rawPos.z);

                r.edgeStartA = new Vector3(oRight, 0, oBottom);
                r.edgeEndA   = new Vector3(oRight, 0, oTop);

                return r;
            }

            // ---------------------------
            // BOTTOM → TOP
            // ---------------------------
            if (Mathf.Abs(mBottom - oTop) <= SnapThreshold &&
                Overlap(mLeft, mRight, oLeft, oRight))
            {
                r.snapped = true;
                r.hasEdge = true;
                r.snappedPos = new Vector3(rawPos.x, rawPos.y, oTop + mHD);

                r.edgeStartA = new Vector3(oLeft, 0, oTop);
                r.edgeEndA   = new Vector3(oRight, 0, oTop);

                return r;
            }

            // ---------------------------
            // TOP → BOTTOM
            // ---------------------------
            if (Mathf.Abs(mTop - oBottom) <= SnapThreshold &&
                Overlap(mLeft, mRight, oLeft, oRight))
            {
                r.snapped = true;
                r.hasEdge = true;
                r.snappedPos = new Vector3(rawPos.x, rawPos.y, oBottom - mHD);

                r.edgeStartA = new Vector3(oLeft, 0, oBottom);
                r.edgeEndA   = new Vector3(oRight, 0, oBottom);

                return r;
            }
        }

        return r;
    }

    // ============================================================
    // 겹침 방지
    // ============================================================
    private static bool CanMoveWithoutOverlap(Vector3 pos, FloorPiece moving)
    {
        Bounds future = new Bounds(pos, moving.GetBounds().size);

        foreach (var other in RoomManager.Instance.GetAllPieces())
        {
            if (other == null || other == moving) continue;

            Bounds o = other.GetBounds();

            bool overlapX = future.max.x > o.min.x && future.min.x < o.max.x;
            bool overlapZ = future.max.z > o.min.z && future.min.z < o.max.z;

            if (overlapX && overlapZ)
                return false;
        }
        return true;
    }

    // ============================================================
    // 그리드 스냅
    // ============================================================
    public static Vector3 SnapToGrid(Vector3 pos)
    {
        pos.x = Mathf.Round(pos.x / GridUnit) * GridUnit;
        pos.z = Mathf.Round(pos.z / GridUnit) * GridUnit;
        return pos;
    }

    private static bool Overlap(float aMin, float aMax, float bMin, float bMax)
    {
        return !(aMax < bMin || aMin > bMax);
    }
}
