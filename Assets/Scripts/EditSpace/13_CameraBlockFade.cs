using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CameraBlockFade : MonoBehaviour
{
    public static CameraBlockFade Instance {get; private set;}
    [Header("Fade Settings")]
    public float fadeAlpha = 0.2f;         // 투명 알파
    public float normalAlpha = 1f;         // 기본 알파
    public float maxFadeDistance = 10f;    // 이 거리 안에 있어야만 투명화

    [Header("Ray Settings")]
    public int rayGridSize = 5;            // 화면 그리드 사이즈 (3 → 3×3, 5 → 5×5)
    public float rayDistance = 50f;        // 레이 탐색 거리

    [Header("Target Camera")]
    public Camera cam;

    private Renderer rend;
    private Material mat;
    // private Camera cam;

    private static HashSet<CameraBlockFade> allWalls = new HashSet<CameraBlockFade>();
    private static HashSet<CameraBlockFade> hitWalls = new HashSet<CameraBlockFade>();

    private void Awake()
    {
        Instance = this;
        rend = GetComponent<Renderer>();
        mat = rend.material; 
        allWalls.Add(this);
    }

    private void OnDestroy()
    {
        allWalls.Remove(this);
    }

    private void LateUpdate()
    {
        // ------------------------------
        // 1) 2D 모드에서는 스크립트 완전 비활성화
        // ------------------------------
        if (EditorModeManager.Instance != null &&
            EditorModeManager.Instance.CurrentMode == EditMode.DrawFloor)
        {
            RestoreAllWalls();
            return;
        }

        // 🔵 카메라가 방 안(바닥 위)에 있으면: 페이드 기능 끄기
        if (IsCameraInsideRoom())
        {
            RestoreAllWalls();
            return;
        }

        // ------------------------------
        // 3) 멀티 레이캐스트 처리
        // ------------------------------
        hitWalls.Clear();

        float step = 1f / (rayGridSize + 1);

        for (int x = 1; x <= rayGridSize; x++)
        {
            for (int y = 1; y <= rayGridSize; y++)
            {
                Vector3 viewport = new Vector3(x * step, y * step, 0);

                Ray ray = cam.ViewportPointToRay(viewport);

                if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
                {
                    var wall = hit.collider.GetComponent<CameraBlockFade>();
                    if (wall != null)
                    {
                        // 카메라와 벽 사이 거리 체크
                        float dist = Vector3.Distance(cam.transform.position, hit.point);
                        if (dist <= wall.maxFadeDistance)
                            hitWalls.Add(wall);
                    }
                }
            }
        }

        // ------------------------------
        // 4) 충돌된 벽 = 투명
        // ------------------------------
        foreach (var wall in hitWalls)
            wall.SetAlpha(wall.fadeAlpha);

        // ------------------------------
        // 5) 충돌 안된 벽 = 복귀
        // ------------------------------
        foreach (var wall in allWalls)
        {
            if (!hitWalls.Contains(wall))
                wall.SetAlpha(wall.normalAlpha);
        }
    }

    public void SetCamera(Camera newCam) => cam = newCam;

    public void RestoreAllWalls()
    {
        foreach (var wall in allWalls)
            wall.SetAlpha(wall.normalAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;
    }

    private bool IsCameraInsideRoom()
    {
        if (RoomManager.Instance == null || cam == null)
            return false;

        Vector3 origin = cam.transform.position;
        Ray ray = new Ray(origin, Vector3.down);

        // 바닥까지 적당한 거리 (필요하면 조절)
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // FloorPiece에 맞았으면 방 안이라고 간주
            var floor = hit.collider.GetComponent<FloorPiece>();
            if (floor != null)
                return true;
        }

        return false;
    }

    public static IEnumerable<CameraBlockFade> GetAllWalls()
    {
        return allWalls;
    }
}
