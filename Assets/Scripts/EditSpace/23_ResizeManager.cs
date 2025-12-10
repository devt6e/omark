using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FloorPiece 리사이즈 전담 매니저
/// - cm 단위 입력 → 10cm 단위 반올림 → meter 변환
/// - 코너 스냅(anchor corner) 유지한 채 크기 조정
/// - 크기 변경으로 인해 경계가 이동한 방향에 인접한 FloorPiece 이동
/// </summary>
public class ResizeManager : MonoBehaviour
{
    public static ResizeManager Instance;

    [Header("Settings")]
    [Tooltip("이웃 FloorPiece 인접 판정 허용 오차")]
    public float edgeTolerance = 0.01f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ============================================================
    // 외부에서 호출하는 메인 API
    // ============================================================
    public void ApplyResize(FloorPiece target, float widthCm, float heightCm)
    {
        if (target == null) return;
        if (RoomManager.Instance == null) return;

        // --- cm → 10cm 단위 반올림 ---
        float roundedWidthCm  = RoundCmToGrid(widthCm);
        float roundedHeightCm = RoundCmToGrid(heightCm);

        // --- cm → meter 변환 ---
        float widthM  = Mathf.Max(SnapUtil.GridUnit, roundedWidthCm  / 100f);
        float heightM = Mathf.Max(SnapUtil.GridUnit, roundedHeightCm / 100f);

        // --- 기존 Bounds ---
        Bounds oldB = target.GetBounds();
        float oldWidth  = oldB.size.x;
        float oldHeight = oldB.size.z;

        // 변화 없으면 종료
        if (Mathf.Approximately(oldWidth, widthM) &&
            Mathf.Approximately(oldHeight, heightM))
        {
            return;
        }

        // --- Anchor Corner 판정 ---
        AnchorCorner anchor = DetermineAnchorCorner(target, oldB);
        Vector3 anchorPos = GetCornerWorldPos(oldB, anchor);

        // --- 새 scale 계산 ---
        Vector3 newScale = target.transform.localScale;
        newScale.x = widthM;
        newScale.z = heightM;

        // --- 새 center 계산 ---
        Vector3 newCenter = ComputeCenterFromAnchor(anchor, anchorPos, widthM, heightM, oldB.center.y);

        // --- 실제 적용 ---
        target.transform.position   = new Vector3(newCenter.x, target.transform.position.y, newCenter.z);
        target.transform.localScale = newScale;

        // --- 적용 후 Bounds ---
        Bounds newB = target.GetBounds();

        // --- 인접 FloorPiece 이동 ---
        MoveNeighborsForResize(target, oldB, newB);

        // (SizeUIController 제거됨 → UI 갱신 없음)
    }

    // ============================================================
    // cm → 10cm 단위 반올림
    // ============================================================
    private float RoundCmToGrid(float cm)
    {
        float grid = SnapUtil.GridUnit * 100f; // 0.1m → 10cm
        if (grid <= 0f) grid = 10f;

        return Mathf.Round(cm / grid) * grid;
    }

    // ============================================================
    // Anchor Corner 판정 (코너 스냅 기반)
    // ============================================================
    private AnchorCorner DetermineAnchorCorner(FloorPiece target, Bounds b)
    {
        List<FloorPiece> all = RoomManager.Instance.GetAllPieces();
        if (all == null || all.Count == 0) return AnchorCorner.Center;

        float halfW = b.size.x * 0.5f;
        float halfD = b.size.z * 0.5f;
        Vector3 c = b.center;

        Vector3[] myCorners =
        {
            new Vector3(c.x - halfW, 0, c.z + halfD), // LT
            new Vector3(c.x - halfW, 0, c.z - halfD), // LB
            new Vector3(c.x + halfW, 0, c.z + halfD), // RT
            new Vector3(c.x + halfW, 0, c.z - halfD), // RB
        };

        foreach (var other in all)
        {
            if (other == null || other == target) continue;

            Bounds ob = other.GetBounds();
            float oHW = ob.size.x * 0.5f;
            float oHD = ob.size.z * 0.5f;
            Vector3 oc = ob.center;

            Vector3[] otherCorners =
            {
                new Vector3(oc.x - oHW, 0, oc.z + oHD),
                new Vector3(oc.x - oHW, 0, oc.z - oHD),
                new Vector3(oc.x + oHW, 0, oc.z + oHD),
                new Vector3(oc.x + oHW, 0, oc.z - oHD)
            };

            for (int i = 0; i < 4; i++)
            {
                Vector3 mc = myCorners[i];
                for (int j = 0; j < 4; j++)
                {
                    float dist = Vector3.Distance(mc, otherCorners[j]);
                    if (dist <= SnapUtil.CornerThreshold)
                    {
                        // anchor corner 확정
                        switch (i)
                        {
                            case 0: return AnchorCorner.LeftTop;
                            case 1: return AnchorCorner.LeftBottom;
                            case 2: return AnchorCorner.RightTop;
                            case 3: return AnchorCorner.RightBottom;
                        }
                    }
                }
            }
        }

        return AnchorCorner.Center;
    }

    // ============================================================
    // 특정 Anchor Corner의 세계 좌표
    // ============================================================
    private Vector3 GetCornerWorldPos(Bounds b, AnchorCorner corner)
    {
        float hw = b.size.x * 0.5f;
        float hd = b.size.z * 0.5f;
        Vector3 c = b.center;

        return corner switch
        {
            AnchorCorner.LeftTop     => new Vector3(c.x - hw, 0, c.z + hd),
            AnchorCorner.LeftBottom  => new Vector3(c.x - hw, 0, c.z - hd),
            AnchorCorner.RightTop    => new Vector3(c.x + hw, 0, c.z + hd),
            AnchorCorner.RightBottom => new Vector3(c.x + hw, 0, c.z - hd),
            _                        => c,
        };
    }

    // ============================================================
    // Anchor 고정 후 새 center 계산
    // ============================================================
    private Vector3 ComputeCenterFromAnchor(AnchorCorner anchor, Vector3 anchorPos, float w, float h, float y)
    {
        float halfW = w * 0.5f;
        float halfD = h * 0.5f;

        return anchor switch
        {
            AnchorCorner.LeftTop     => new Vector3(anchorPos.x + halfW, y, anchorPos.z - halfD),
            AnchorCorner.LeftBottom  => new Vector3(anchorPos.x + halfW, y, anchorPos.z + halfD),
            AnchorCorner.RightTop    => new Vector3(anchorPos.x - halfW, y, anchorPos.z - halfD),
            AnchorCorner.RightBottom => new Vector3(anchorPos.x - halfW, y, anchorPos.z + halfD),
            _                        => new Vector3(anchorPos.x, y, anchorPos.z),
        };
    }

    // ============================================================
    // 크기 변경 후 인접 FloorPiece 이동
    // ============================================================
    private void MoveNeighborsForResize(FloorPiece target, Bounds oldB, Bounds newB)
    {
        List<FloorPiece> all = RoomManager.Instance.GetAllPieces();
        if (all == null) return;

        float oldLeft   = oldB.min.x;
        float oldRight  = oldB.max.x;
        float oldBottom = oldB.min.z;
        float oldTop    = oldB.max.z;

        float newLeft   = newB.min.x;
        float newRight  = newB.max.x;
        float newBottom = newB.min.z;
        float newTop    = newB.max.z;

        float dLeft   = newLeft   - oldLeft;
        float dRight  = newRight  - oldRight;
        float dBottom = newBottom - oldBottom;
        float dTop    = newTop    - oldTop;

        foreach (var p in all)
        {
            if (p == null || p == target) continue;

            Bounds pb = p.GetBounds();

            // --- RIGHT edge 이동에 붙어 있었던 이웃 이동 ---
            if (Mathf.Abs(dRight) > Mathf.Epsilon &&
                Mathf.Abs(pb.min.x - oldRight) <= edgeTolerance &&
                IsOverlap(oldBottom, oldTop, pb.min.z, pb.max.z))
            {
                p.transform.position += new Vector3(dRight, 0, 0);
            }

            // --- LEFT edge 이동 ---
            if (Mathf.Abs(dLeft) > Mathf.Epsilon &&
                Mathf.Abs(pb.max.x - oldLeft) <= edgeTolerance &&
                IsOverlap(oldBottom, oldTop, pb.min.z, pb.max.z))
            {
                p.transform.position += new Vector3(dLeft, 0, 0);
            }

            // --- TOP edge 이동 ---
            if (Mathf.Abs(dTop) > Mathf.Epsilon &&
                Mathf.Abs(pb.min.z - oldTop) <= edgeTolerance &&
                IsOverlap(oldLeft, oldRight, pb.min.x, pb.max.x))
            {
                p.transform.position += new Vector3(0, 0, dTop);
            }

            // --- BOTTOM edge 이동 ---
            if (Mathf.Abs(dBottom) > Mathf.Epsilon &&
                Mathf.Abs(pb.max.z - oldBottom) <= edgeTolerance &&
                IsOverlap(oldLeft, oldRight, pb.min.x, pb.max.x))
            {
                p.transform.position += new Vector3(0, 0, dBottom);
            }
        }
    }

    private bool IsOverlap(float aMin, float aMax, float bMin, float bMax)
    {
        return !(aMax <= bMin || aMin >= bMax);
    }
}

// ============================================================
// Anchor Corner 정의
// ============================================================
public enum AnchorCorner
{
    Center,
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom
}
