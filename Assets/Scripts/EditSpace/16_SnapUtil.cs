using System.Collections.Generic;
using UnityEngine;

public static class SnapUtil
{
    // 기본 그리기 단위(10cm = 0.1m)
    public const float UNIT = 0.1f;

    // 근처 FloorPiece 변에 붙는 스냅 거리(예: 15cm)
    public const float SNAP_THRESHOLD = 0.15f;

    // ================================
    // 10cm 단위 스냅
    // ================================
    public static float SnapToUnit(float v)
    {
        return Mathf.Round(v / UNIT) * UNIT;
    }

    public static Vector3 SnapToUnit(Vector3 v)
    {
        v.x = SnapToUnit(v.x);
        v.z = SnapToUnit(v.z);
        return v;
    }

    // ================================
    // FloorPiece 경계 스냅
    // (startPoint, currentPoint 모두 사용)
    // ================================
    public static Vector3 SnapToFloorEdges(Vector3 pos, List<FloorPiece> pieces)
    {
        float px = pos.x;
        float pz = pos.z;

        foreach (var f in pieces)
        {
            if (f == null) continue;

            Bounds b = f.GetBounds();
            float minX = b.min.x;
            float maxX = b.max.x;
            float minZ = b.min.z;
            float maxZ = b.max.z;

            // ----- X 방향 스냅 -----
            if (Mathf.Abs(px - minX) <= SNAP_THRESHOLD) px = minX;
            if (Mathf.Abs(px - maxX) <= SNAP_THRESHOLD) px = maxX;

            // ----- Z 방향 스냅 -----
            if (Mathf.Abs(pz - minZ) <= SNAP_THRESHOLD) pz = minZ;
            if (Mathf.Abs(pz - maxZ) <= SNAP_THRESHOLD) pz = maxZ;
        }

        return new Vector3(px, pos.y, pz);
    }

    // ================================
    // 시작/끝점 보정 (ObjectSnap → UnitSnap 순)
    // ================================
    public static Vector3 CleanPosition(Vector3 raw, List<FloorPiece> pieces)
    {
        // 1) 먼저 FloorPiece 경계에 붙일지 확인
        Vector3 snapped = SnapToFloorEdges(raw, pieces);

        // 2) 마지막으로 10cm 단위로 정리
        snapped = SnapToUnit(snapped);

        return snapped;
    }
}
