using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CameraBlockFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeAlpha = 0.2f;         // 투명 알파
    public float normalAlpha = 1f;         // 기본 알파
    public float maxFadeDistance = 10f;    // 이 거리 안에 있어야만 투명화

    [Header("Ray Settings")]
    public int rayGridSize = 5;            // 화면 그리드 사이즈 (3 → 3×3, 5 → 5×5)
    public float rayDistance = 50f;        // 레이 탐색 거리

    private Renderer rend;
    private Material mat;
    private Camera cam;

    private static HashSet<CameraBlockFade> allWalls = new HashSet<CameraBlockFade>();
    private static HashSet<CameraBlockFade> hitWalls = new HashSet<CameraBlockFade>();

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material; 
        allWalls.Add(this);
    }

    private void OnDestroy()
    {
        allWalls.Remove(this);
    }

    private void Start()
    {
        cam = Camera.main;
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

        if (EditorModeManager.Instance != null &&
            EditorModeManager.Instance.CurrentMode == EditMode.MoveView2D)
        {
            // MoveView는 2D 카메라 모드일 가능성 있음 → 카메라 타입 체크
            if (cam != null && cam.orthographic)
            {
                RestoreAllWalls();
                return;
            }
        }

        // ------------------------------
        // 2) 카메라 찾기
        // ------------------------------
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

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

    private void RestoreAllWalls()
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
}
