using UnityEngine;
using System.IO;

public class RoomLoader3D : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floorPiecePrefab;   // FloorPiece 컴포넌트가 붙어 있어야 함

    [Header("References")]
    public RoomManager roomManager;
    public WallGenerator wallGenerator;

    private void Start()
    {
        LoadRoom();
    }

    private void LoadRoom()
    {
        // 1) JSON 경로 결정
        string path = RoomLoadContext.loadFilePath;

        if (string.IsNullOrEmpty(path))
            path = Path.Combine(Application.persistentDataPath, "floorplan.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("RoomLoader3D: JSON 파일이 없습니다. path=" + path);
            return;
        }

        // 2) JSON 읽기
        string json = File.ReadAllText(path);
        FloorplanData data = JsonUtility.FromJson<FloorplanData>(json);

        if (data == null || data.pieces == null)
        {
            Debug.LogError("RoomLoader3D: JSON 파싱 실패");
            return;
        }

        Debug.Log($"RoomLoader3D: JSON 불러오기 성공. floor count = {data.pieces.Length}");

        // 3) 기존 FloorPiece 있으면 제거
        ClearExistingFloorPieces();

        // 4) FloorPiece 생성 + RoomManager 등록
        foreach (var fpData in data.pieces)
        {
            GameObject obj = Instantiate(floorPiecePrefab);
            FloorPiece fp = obj.GetComponent<FloorPiece>();

            fp.FromData(fpData);
            roomManager.RegisterPiece(fp);
        }

        // 5) 벽 재생성
        if (wallGenerator != null)
            wallGenerator.RegenerateWalls();
        else
            Debug.LogWarning("RoomLoader3D: WallGenerator가 없습니다.");

        // 6) 카메라 자동 중심 이동 알림
        NotifyCameraToReposition();

        Debug.Log("RoomLoader3D: 방 불러오기 완료");
    }

    // ======================================
    // 기존 FloorPiece 제거
    // ======================================
    private void ClearExistingFloorPieces()
    {
        var existingPieces = FindObjectsByType<FloorPiece>(FindObjectsSortMode.None);

        foreach (var p in existingPieces)
        {
            roomManager.UnregisterPiece(p);
            Destroy(p.gameObject);
        }
    }

    // ======================================
    // 카메라 재배치 요청
    // ======================================
    private void NotifyCameraToReposition()
    {
        // Camera3DController가 자신을 초기화하도록 해도 되고,
        // ViewModeController 방식이면 Set3DView()가 알아서 처리함.
        
        // ViewModeCameraAnchor anchor = FindAnyObjectByType<ViewModeCameraAnchor>();
        // if (anchor != null)
        //     anchor.RepositionCamera();
    }
}
