using UnityEngine;

public class FurnitureSpawner3D : MonoBehaviour
{
    public static FurnitureSpawner3D Instance { get; private set; }

    [Header("Prefab")]
    public GameObject furniturePrefab;

    [Header("LayerMask")]
    public LayerMask floorMask; // 바닥에 레이캐스트할 때 사용

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    /// <summary>
    /// UI 팝업에서 입력받은 사이즈로 가구 생성
    /// </summary>
    public void SpawnFurniture(Vector3 size)
    {
        Vector3 spawnPos = GetSpawnPosition(size);

        GameObject obj = Instantiate(furniturePrefab, spawnPos, Quaternion.identity);
        FurniturePiece piece = obj.GetComponent<FurniturePiece>();

        piece.Initialize(size);
        FurnitureManager.Instance.Register(piece);
    }

    /// <summary>
    /// 생성 위치: 카메라 앞 → 바닥으로 레이캐스트
    /// </summary>
    private Vector3 GetSpawnPosition(Vector3 size)
    {
        Vector3 origin = mainCam.transform.position;
        Vector3 dir = mainCam.transform.forward;

        // if (Physics.Raycast(origin, dir, out RaycastHit hit, 100f, floorMask))
        // {
        //     float y = size.y * 0.5f;
        //     return new Vector3(hit.point.x, y, hit.point.z);
        // }

        // fallback: 카메라 앞 0.5f 지점
        Vector3 fallback = origin + dir * 0.5f;
        fallback.y = size.y * 0.5f;
        return fallback;
    }
}
