using UnityEngine;
using System.Collections;

public class ViewModeController : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject camera2DObj;
    public GameObject camera3DObj;

    [Header("References")]
    public GameObject grid2D;
    public WallGenerator wallGenerator;

    [Header("UI Roots")]
    public GameObject ui2D;
    public GameObject ui3D;

    [Header("View Buttons")]
    public UnityEngine.UI.Image button2D;
    public UnityEngine.UI.Image button3D;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.6f;

    private bool is3DView = false;

    private void Start()
    {
        GlobalCameraManager.Camera2D = camera2DObj.GetComponent<Camera>();
        GlobalCameraManager.Camera3D = camera3DObj.GetComponent<Camera>();;
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

        if (camera2DObj) camera2DObj.SetActive(true);
        if (camera3DObj) camera3DObj.SetActive(false);

        if (grid2D) grid2D.SetActive(true);

        if (wallGenerator?.wallsRoot != null)
            wallGenerator.wallsRoot.gameObject.SetActive(false);

        // UI 전환
        if (ui2D) ui2D.SetActive(true);
        if (ui3D) ui3D.SetActive(false);

        // 기본 모드 설정
        EditorModeManager.Instance.SetMode(EditMode.MoveView2D);
        ModeUIController2D.Instance.UpdateUI(EditMode.MoveView2D);

        foreach (var wall in CameraBlockFade.GetAllWalls())
        {
            wall.SetCamera(null);
            wall.RestoreAllWalls();
        }

        SelectionManager.Instance?.SetCamera(GlobalCameraManager.Camera2D);
        FloorDrawer.Instance?.SetCamera(GlobalCameraManager.Camera2D);
        CameraMoveController.Instance?.SetCamera(GlobalCameraManager.Camera2D);

        UpdateButtonVisual();
    }

    private void Set3DView()
    {
        Debug.Log("3D " + GlobalCameraManager.Camera3D);
        is3DView = true;

        if (camera2DObj) camera2DObj.SetActive(false);
        if (camera3DObj) camera3DObj.SetActive(true);

        if (grid2D) grid2D.SetActive(false);

        if (wallGenerator?.wallsRoot != null)
            wallGenerator.wallsRoot.gameObject.SetActive(true);

        wallGenerator?.RegenerateWalls();

        var cam3D = camera3DObj.GetComponent<Camera>();
        foreach (var wall in CameraBlockFade.GetAllWalls())
            wall.cam = cam3D;

        CameraBlockFade.Instance?.SetCamera(GlobalCameraManager.Camera3D);
        Camera3DController.Instance?.SetCamera(GlobalCameraManager.Camera3D);

        Position3DCamera();

        // UI 전환
        if (ui2D) ui2D.SetActive(false);
        if (ui3D) ui3D.SetActive(true);

        // 기본 모드 설정
        EditorModeManager.Instance.SetMode(EditMode.MoveView3D);
        ModeUIController3D.Instance.UpdateUI(EditMode.MoveView3D);

        UpdateButtonVisual();
    }

    private void Position3DCamera()
    {
        Bounds roomBounds = RoomManager.Instance.GetRoomBounds();

        if (roomBounds.size == Vector3.zero)
        {
            camera3DObj.transform.position = new Vector3(0, 10, -10);
            camera3DObj.transform.LookAt(Vector3.zero);
            return;
        }

        Vector3 center = roomBounds.center;
        float maxSize = Mathf.Max(roomBounds.size.x, roomBounds.size.z);
        float distance = Mathf.Clamp(maxSize * 1.5f, 8f, 60f);

        Vector3 camOffset = new Vector3(-distance * 0.6f, distance, -distance);
        // Vector3 camOffset = new Vector3(-distance * 0.6f, 1.5f, -distance);
        // Vector3 camOffset = new Vector3(-distance * 0.6f, distance, 0f);
        // camOffset = Vector3.zero;

        camera3DObj.transform.position = center + camOffset;
        // camera3DObj.transform.position = center;
        camera3DObj.transform.LookAt(center);
    }

    private void UpdateButtonVisual()
    {
        if (button2D)
        {
            Color c = button2D.color;
            c.a = is3DView ? inactiveAlpha : activeAlpha;
            button2D.color = c;
        }

        if (button3D)
        {
            Color c = button3D.color;
            c.a = is3DView ? activeAlpha : inactiveAlpha;
            button3D.color = c;
        }
    }
}
