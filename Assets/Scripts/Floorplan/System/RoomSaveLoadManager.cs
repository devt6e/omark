using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class FloorSaveLoadManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject floorPiecePrefab; // 반드시 FloorPiece.cs가 붙어야 함

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "floorplan.json");

    // ======================================
    // SAVE
    // ======================================
    public void Save()
    {
        var pieces = FindObjectsByType<FloorPiece>(FindObjectsSortMode.None);

        FloorplanData data = new FloorplanData();
        data.pieces = new FloorPieceData[pieces.Length];

        for (int i = 0; i < pieces.Length; i++)
            data.pieces[i] = pieces[i].ToData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"Floor saved to: {SavePath}");
    }

    // ======================================
    // LOAD
    // ======================================
    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Save file not found.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        FloorplanData data = JsonUtility.FromJson<FloorplanData>(json);

        // 1) 기존 FloorPiece 제거
        var oldPieces = FindObjectsByType<FloorPiece>(FindObjectsSortMode.None);
        foreach (var p in oldPieces)
            Destroy(p.gameObject);

        // 2) 새 FloorPiece 생성
        var loadedPieces = new List<FloorPiece>();

        foreach (var pieceData in data.pieces)
        {
            GameObject obj = Instantiate(floorPiecePrefab, transform);
            FloorPiece fp = obj.GetComponent<FloorPiece>();
            fp.FromData(pieceData);
            loadedPieces.Add(fp);
        }

        Debug.Log("Floor loaded from: " + SavePath);

        // 3) 중앙정렬 로직 실행
        CenterFloorPieces(loadedPieces);
    }

    private void CenterFloorPieces(List<FloorPiece> pieces)
    {
        if (pieces == null || pieces.Count == 0)
            return;

        // 전체 Bounds 계산
        Bounds totalBounds = new Bounds(pieces[0].transform.position, Vector3.zero);

        foreach (var piece in pieces)
            totalBounds.Encapsulate(piece.GetBounds());

        Vector3 currentCenter = totalBounds.center;
        
        // 원하는 중심점 (여기서는 월드 기준 0,0,0)
        Vector3 desiredCenter = Vector3.zero;

        // 이동해야 할 오프셋
        Vector3 offset = desiredCenter - currentCenter;

        // 모든 FloorPiece 이동
        foreach (var piece in pieces)
            piece.transform.position += offset;

        Debug.Log($"Floor centered. Offset applied: {offset}");
    }

}
