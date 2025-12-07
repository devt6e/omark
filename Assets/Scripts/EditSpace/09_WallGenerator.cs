using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject wallPrefab;
    public Transform wallsRoot;      // 생성된 벽들을 담을 부모 오브젝트
    public float wallHeight = 2.5f;  // 벽 높이 (m)
    public float wallThickness = 0.1f; // 벽 두께 (m)
    public float touchTolerance = 0.001f; // FloorPiece 간 접촉 판정 허용 오차

    // 외부에서 호출: 기존 벽 삭제 후 다시 생성
    public void RegenerateWalls()
    {
        ClearWalls();
        GenerateWalls();
    }

    // 기존 벽 전부 삭제
    public void ClearWalls()
    {
        if (wallsRoot == null) return;

        for (int i = wallsRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(wallsRoot.GetChild(i).gameObject);
        }
    }

    private void GenerateWalls()
    {
        if (wallPrefab == null || RoomManager.Instance == null)
        {
            Debug.LogWarning("WallGenerator: wallPrefab 또는 RoomManager가 설정되지 않았습니다.");
            return;
        }

        List<FloorPiece> pieces = RoomManager.Instance.GetAllPieces();

        foreach (var piece in pieces)
        {
            if (piece == null) continue;
            GenerateWallsForPiece(piece, pieces);
        }
    }

    private void GenerateWallsForPiece(FloorPiece piece, List<FloorPiece> allPieces)
    {
        Bounds b = piece.GetBounds();

        float minX = b.min.x;
        float maxX = b.max.x;
        float minZ = b.min.z;
        float maxZ = b.max.z;

        // 각 면에 이웃이 있는지 체크
        bool hasLeftNeighbor   = HasNeighborOnSide(piece, allPieces, Side.Left);
        bool hasRightNeighbor  = HasNeighborOnSide(piece, allPieces, Side.Right);
        bool hasTopNeighbor    = HasNeighborOnSide(piece, allPieces, Side.Top);
        bool hasBottomNeighbor = HasNeighborOnSide(piece, allPieces, Side.Bottom);

        // 각 변에 대해, 이웃이 없다면 외곽 벽 생성

        // Left (x = minX)
        if (!hasLeftNeighbor)
        {
            Vector3 pos = new Vector3(minX - wallThickness * 0.5f, wallHeight * 0.5f, (minZ + maxZ) / 2f);
            Vector3 scale = new Vector3(wallThickness, wallHeight, (maxZ - minZ));
            CreateWall(pos, scale);
        }

        // Right (x = maxX)
        if (!hasRightNeighbor)
        {
            Vector3 pos = new Vector3(maxX + wallThickness * 0.5f, wallHeight * 0.5f, (minZ + maxZ) / 2f);
            Vector3 scale = new Vector3(wallThickness, wallHeight, (maxZ - minZ));
            CreateWall(pos, scale);
        }

        // Top (z = maxZ)
        if (!hasTopNeighbor)
        {
            Vector3 pos = new Vector3((minX + maxX) / 2f, wallHeight * 0.5f, maxZ + wallThickness * 0.5f);
            Vector3 scale = new Vector3((maxX - minX), wallHeight, wallThickness);
            CreateWall(pos, scale);
        }

        // Bottom (z = minZ)
        if (!hasBottomNeighbor)
        {
            Vector3 pos = new Vector3((minX + maxX) / 2f, wallHeight * 0.5f, minZ - wallThickness * 0.5f);
            Vector3 scale = new Vector3((maxX - minX), wallHeight, wallThickness);
            CreateWall(pos, scale);
        }
    }

    private void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, wallsRoot);
        wall.transform.localScale = scale;
    }

    private enum Side { Left, Right, Top, Bottom }

    // 해당 FloorPiece의 특정 면에 이웃 FloorPiece가 붙어 있는지 검사
    private bool HasNeighborOnSide(FloorPiece target, List<FloorPiece> all, Side side)
    {
        Bounds t = target.GetBounds();

        float tMinX = t.min.x;
        float tMaxX = t.max.x;
        float tMinZ = t.min.z;
        float tMaxZ = t.max.z;

        foreach (var other in all)
        {
            if (other == null || other == target) continue;

            Bounds o = other.GetBounds();

            float oMinX = o.min.x;
            float oMaxX = o.max.x;
            float oMinZ = o.min.z;
            float oMaxZ = o.max.z;

            switch (side)
            {
                case Side.Left:
                    // target 왼쪽 변과 other 오른쪽 변이 맞닿고, Z 범위가 겹치는지
                    if (Mathf.Abs(tMinX - oMaxX) <= touchTolerance &&
                        tMaxZ > oMinZ + touchTolerance &&
                        tMinZ < oMaxZ - touchTolerance)
                        return true;
                    break;

                case Side.Right:
                    if (Mathf.Abs(tMaxX - oMinX) <= touchTolerance &&
                        tMaxZ > oMinZ + touchTolerance &&
                        tMinZ < oMaxZ - touchTolerance)
                        return true;
                    break;

                case Side.Top:
                    if (Mathf.Abs(tMaxZ - oMinZ) <= touchTolerance &&
                        tMaxX > oMinX + touchTolerance &&
                        tMinX < oMaxX - touchTolerance)
                        return true;
                    break;

                case Side.Bottom:
                    if (Mathf.Abs(tMinZ - oMaxZ) <= touchTolerance &&
                        tMaxX > oMinX + touchTolerance &&
                        tMinX < oMaxX - touchTolerance)
                        return true;
                    break;
            }
        }

        return false;
    }
}
