using UnityEngine;

public struct SnapResult
{
    public bool snapped;          // 스냅이 발생했는가?
    public Vector3 snappedPos;    // 스냅 후 적용될 위치

    public bool hasEdge;          // 면 스냅 여부
    public bool hasCorner;        // 코너 스냅 여부

    public Vector3 edgeStartA;
    public Vector3 edgeEndA;

    public Vector3 edgeStartB; // 코너 스냅용 두 번째 edge
    public Vector3 edgeEndB;

    public static SnapResult NoSnap(Vector3 raw)
    {
        return new SnapResult
        {
            snapped = false,
            snappedPos = raw,
            hasEdge = false,
            hasCorner = false
        };
    }
}
