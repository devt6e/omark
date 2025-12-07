using UnityEngine;

public class ViewModeController : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject camera2DObj;
    public GameObject camera3DObj;

    [Header("References")]
    public GameObject grid2D;
    public WallGenerator wallGenerator;

    // [Header("UI")]
    // public Canvas uiCanvas;   // UI Canvas(스크린 공간-카메라로 설정된 경우)
    [Header("UI Buttons")]
    public UnityEngine.UI.Image button2D;
    public UnityEngine.UI.Image button3D;
    [SerializeField] private float activeAlpha = 1f;     // 255 / 255
    [SerializeField] private float inactiveAlpha = 0.6f; // 150 / 255


    private bool is3DView = false;

    private void Start()
    {
        Set2DView();
    }

    public void OnClick2D()
    {
        Set2DView();
    }

    public void OnClick3D()
    {
        Set3DView();
    }

    private void Set2DView()
    {
        is3DView = false;

        // if (uiCanvas != null)
        //     uiCanvas.worldCamera = camera2DObj.GetComponent<Camera>();   // ⭐ UI 카메라 변경

        // 카메라 GameObject 전환
        if (camera2DObj != null) camera2DObj.SetActive(true);
        if (camera3DObj != null) camera3DObj.SetActive(false);

        // 2D 전용 UI/그리드 활성화
        if (grid2D != null) grid2D.SetActive(true);

        // 3D 전용 오브젝트 비활성화
        if (wallGenerator != null && wallGenerator.wallsRoot != null)
            wallGenerator.wallsRoot.gameObject.SetActive(false);

        UpdateButtonVisual();
    }

    private void Set3DView()
    {
        is3DView = true;

        // if (uiCanvas != null)
        //     uiCanvas.worldCamera = camera3DObj.GetComponent<Camera>();   // ⭐ UI 카메라 변경

        // 카메라 GameObject 전환
        if (camera2DObj != null) camera2DObj.SetActive(false);
        if (camera3DObj != null) camera3DObj.SetActive(true);

        // 2D 전용 UI/그리드 비활성화
        if (grid2D != null) grid2D.SetActive(false);

        // 3D 전용 오브젝트 활성화 + 벽 재생성
        if (wallGenerator != null)
        {
            if (wallGenerator.wallsRoot != null)
                wallGenerator.wallsRoot.gameObject.SetActive(true);

            wallGenerator.RegenerateWalls();

            // 🔥 추가: 공간 중심을 향해 자동 카메라 위치 조정
            Position3DCamera();
            UpdateButtonVisual();
        }
    }

    private void Position3DCamera()
    {
        Bounds roomBounds = RoomManager.Instance.GetRoomBounds();

        // 방이 없는 경우 기본 위치
        if (roomBounds.size == Vector3.zero)
        {
            camera3DObj.transform.position = new Vector3(0, 10, -10);
            camera3DObj.transform.LookAt(Vector3.zero);
            return;
        }

        Vector3 center = roomBounds.center;

        float maxSize = Mathf.Max(roomBounds.size.x, roomBounds.size.z);

        // 거리 계산 (방이 클수록 멀어진다)
        float distance = Mathf.Clamp(maxSize * 1.5f, 8f, 60f);

        // 카메라 기본 시점 (약간 위에서 비스듬히 내려다보는 각도)
        Vector3 camOffset = new Vector3(-distance * 0.6f, distance, -distance);

        camera3DObj.transform.position = center + camOffset;
        camera3DObj.transform.LookAt(center);
    }

    private void UpdateButtonVisual()
    {
        if (button2D != null)
        {
            Color c = button2D.color;
            c.a = is3DView ? inactiveAlpha : activeAlpha;
            button2D.color = c;
        }

        if (button3D != null)
        {
            Color c = button3D.color;
            c.a = is3DView ? activeAlpha : inactiveAlpha;
            button3D.color = c;
        }
    }

}
