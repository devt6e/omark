using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FloorDrawer : MonoBehaviour
{
    public static FloorDrawer Instance { get; private set; }

    [Header("Input Actions")]
    public InputActionReference pointAction;   // Pointer position (mouse/touch)
    public InputActionReference contactAction;   // Pointer press

    [Header("Prefabs")]
    public GameObject floorPreviewPrefab;
    public GameObject floorFinalPrefab;

    [Header("Dimension UI")]
    public Canvas uiCanvas;
    public DimensionLabelUI dimensionLabelPrefab;

    private DimensionLabelUI widthLabel;
    private DimensionLabelUI heightLabel;

    private GameObject previewObj;
    private bool isDragging = false;
    private Vector3 dragStartPos;

    public Camera cam;

    private void Awake()
    {
        Instance = this;
        EnableInput();
    }

    private void OnEnable()
    {
        contactAction.action.started += OnClickStarted;
        contactAction.action.canceled += OnClickCanceled;
    }

    private void OnDisable()
    {
        contactAction.action.started -= OnClickStarted;
        contactAction.action.canceled -= OnClickCanceled;
    }

    private void Update()
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.DrawFloor)
            return;

        if (!isDragging)
            return;

        Vector3 current = GetMouseWorldPos();

        // ← 필요하면 여기를 활성화하면 10cm 단위 드로잉 가능
        current = SnapUtil.SnapToGrid(current);

        UpdatePreview(current);
    }
    public void SetCamera(Camera newCam) => cam = newCam;
    // ============================================================
    // 클릭 시작
    // ============================================================
    private void OnClickStarted(InputAction.CallbackContext ctx)
    {
        // Debug.Log("OnPressed");
        if (EditorModeManager.Instance.CurrentMode != EditMode.DrawFloor)
            return;

        if (IsPointerOverUI())
            return;

        // 시작 지점을 동일하게 보정하고 싶으면 이 라인 사용
        dragStartPos = GetMouseWorldPos();
        dragStartPos = SnapUtil.SnapToGrid(dragStartPos); // optional

        StartPreview();
    }

    // ============================================================
    // 클릭 종료
    // ============================================================
    private void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.DrawFloor)
            return;

        if (isDragging)
            EndPreview();
    }

    // ============================================================
    // WorldPos 계산
    // ============================================================
    private Vector3 GetMouseWorldPos()
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }

    // ============================================================
    // Preview 시작
    // ============================================================
    private void StartPreview()
    {
        isDragging = true;
        previewObj = Object.Instantiate(floorPreviewPrefab);

        widthLabel  = Object.Instantiate(dimensionLabelPrefab, uiCanvas.transform);
        heightLabel = Object.Instantiate(dimensionLabelPrefab, uiCanvas.transform);
    }

    // ============================================================
    // 드래그 중 실시간 미리보기 업데이트
    // ============================================================
    private void UpdatePreview(Vector3 current)
    {
        Vector3 center = (dragStartPos + current) * 0.5f;
        float width = Mathf.Abs(current.x - dragStartPos.x);
        float depth = Mathf.Abs(current.z - dragStartPos.z);

        // 최소 단위 제한
        float minUnit = SnapUtil.GridUnit;
        if (width < minUnit) width = minUnit;
        if (depth < minUnit) depth = minUnit;

        previewObj.transform.position = center;
        previewObj.transform.localScale = new Vector3(width, 0.1f, depth);

        UpdateDimensionPreview(center, width, depth);   
    }

    // ============================================================
    // 드래그 종료 → 최종 FloorPiece 생성
    // ============================================================
    private void EndPreview()
    {
        isDragging = false;

        // UI 삭제
        if (widthLabel != null) Object.Destroy(widthLabel.gameObject);
        if (heightLabel != null) Object.Destroy(heightLabel.gameObject);
        widthLabel = null;
        heightLabel = null;

        if (previewObj == null) return;

        Vector3 pos   = previewObj.transform.position;
        Vector3 scale = previewObj.transform.localScale;
        Bounds candidateBounds = new Bounds(pos, scale);

        Object.Destroy(previewObj);
        previewObj = null;

        // RoomManager 규칙 검사
        bool canPlace = RoomManager.Instance != null &&
                        RoomManager.Instance.CanPlace(candidateBounds);

        if (!canPlace)
        {
            Debug.Log("FloorDrawer: 연결되지 않은 위치이므로 배치 불가.");
            return;
        }

        // ============================================
        // FloorPiece 생성
        // ============================================
        GameObject finalFloor = Object.Instantiate(floorFinalPrefab);
        finalFloor.transform.position = pos;
        finalFloor.transform.localScale = scale;

        FloorPiece piece = finalFloor.GetComponent<FloorPiece>();
        if (piece == null)
            piece = finalFloor.AddComponent<FloorPiece>();

        // ============================================
        // 🔥 생성 시에도 피스 스냅 적용
        // ============================================
        pos = SnapUtil.SnapFloorPiecePosition(pos, piece, SnapUtil.SnapThreshold);
        finalFloor.transform.position = pos;

        RoomManager.Instance.RegisterPiece(piece);

        DimensionLabelUIManager.Instance.HidePreview();
    }


    // ============================================================
    // UI Label 업데이트
    // ============================================================
    private void UpdateDimensionPreview(Vector3 center, float width, float depth)
    {
        var mgr = DimensionLabelUIManager.Instance;

        // 가로 (위쪽)
        mgr.widthPreviewLabel.placeAbove = true;
        mgr.widthPreviewLabel.placeRight = false;
        mgr.widthPreviewLabel.SetWorldLabel(
            new Vector3(center.x, 1f, center.z + depth * 0.5f),
            width
        );

        // 세로 (오른쪽)
        mgr.heightPreviewLabel.placeAbove = false;
        mgr.heightPreviewLabel.placeRight = true;
        mgr.heightPreviewLabel.SetWorldLabel(
            new Vector3(center.x + width * 0.5f, 1f, center.z),
            depth
        );
    }

    // ============================================================
    // UI 체크
    // ============================================================
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (Mouse.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Touchscreen.current != null)
        {
            foreach (var t in Touchscreen.current.touches)
            {
                if (t.isInProgress && EventSystem.current.IsPointerOverGameObject(t.touchId.ReadValue()))
                    return true;
            }
        }

        return false;
    }

    public void EnableInput()
    {
        pointAction.action.Enable();
        contactAction.action.Enable();
    }

    public void DisableInput()
    {
        pointAction.action.Disable();
        contactAction.action.Disable();
    }
}
