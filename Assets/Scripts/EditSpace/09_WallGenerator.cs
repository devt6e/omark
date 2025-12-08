using System.Collections.Generic;
using UnityEngine;
using Clipper2Lib;  // Clipper2 C# 라이브러리

public class WallGenerator : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject wallPrefab;
    public Transform wallsRoot;
    public float wallHeight = 2.5f;
    public float wallThickness = 0.1f;

    [Header("Clipper Settings")]
    [Tooltip("float -> int64 변환 스케일 (너무 작으면 정밀도 떨어짐)")]
    public double scale = 1000.0;

    // ============================================
    // 외부에서 호출: 기존 벽 삭제 후 다시 생성
    // ============================================
    public void RegenerateWalls()
    {
        ClearWalls();
        GenerateWallsBoolean();
    }

    // ============================================
    // 기존 벽 삭제
    // ============================================
    private void ClearWalls()
    {
        if (wallsRoot == null) return;

        for (int i = wallsRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(wallsRoot.GetChild(i).gameObject);
        }
    }

    // ============================================
    // Boolean 기반 벽 생성 메인
    // ============================================
    private void GenerateWallsBoolean()
    {
        if (wallPrefab == null)
        {
            Debug.LogWarning("WallGenerator: wallPrefab이 비어있습니다.");
            return;
        }

        if (RoomManager.Instance == null)
        {
            Debug.LogWarning("WallGenerator: RoomManager.Instance가 없습니다.");
            return;
        }

        List<FloorPiece> pieces = RoomManager.Instance.GetAllPieces();
        if (pieces == null || pieces.Count == 0)
        {
            // 생성할 바닥이 없으면 벽도 없음
            return;
        }

        // 1) FloorPiece들의 rect를 Clipper용 Paths64로 변환
        Paths64 subject = new Paths64();

        foreach (var piece in pieces)
        {
            if (piece == null) continue;

            Bounds b = piece.GetBounds();

            double x1 = b.min.x * scale;
            double x2 = b.max.x * scale;
            double z1 = b.min.z * scale;
            double z2 = b.max.z * scale;

            Path64 rect = new Path64
            {
                new Point64((long)x1, (long)z1),
                new Point64((long)x2, (long)z1),
                new Point64((long)x2, (long)z2),
                new Point64((long)x1, (long)z2)
            };

            subject.Add(rect);
        }

        if (subject.Count == 0)
            return;

        // 2) Boolean Union으로 전체 외곽선 계산
        // FillRule은 방이 겹쳐도 한 덩어리로 보는 NonZero 또는 EvenOdd 사용
        Paths64 solution = Clipper.Union(subject, FillRule.NonZero);

        if (solution == null || solution.Count == 0)
            return;

        // 3) 각 외곽 Path를 따라 벽 생성
        foreach (Path64 path in solution)
        {
            CreateWallsFromPath(path);
        }
    }

    // ============================================
    // 하나의 외곽 Path64를 따라 벽 생성
    // ============================================
    private void CreateWallsFromPath(Path64 path)
    {
        int count = path.Count;
        if (count < 2) return;

        for (int i = 0; i < count; i++)
        {
            Point64 a = path[i];
            Point64 b = path[(i + 1) % count]; // 루프 연결

            Vector3 p1 = new Vector3(
                (float)(a.X / scale),
                0f,
                (float)(a.Y / scale)
            );

            Vector3 p2 = new Vector3(
                (float)(b.X / scale),
                0f,
                (float)(b.Y / scale)
            );

            CreateWallSegment(p1, p2);
        }
    }

    // ============================================
    // 단일 벽 세그먼트 생성
    // ============================================
    private void CreateWallSegment(Vector3 p1, Vector3 p2)
    {
        Vector3 dir = p2 - p1;
        float length = dir.magnitude;
        if (length < 0.001f) return; // 너무 짧은 건 무시

        Vector3 center = (p1 + p2) * 0.5f + new Vector3(0f,wallHeight/2, 0f);
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject wall = Instantiate(wallPrefab, center, rot, wallsRoot);
        wall.transform.localScale = new Vector3(wallThickness, wallHeight, length);
    }
}
