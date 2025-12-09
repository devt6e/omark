using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FloorDrawer : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference pointAction;   // Pointer position (mouse/touch)
    public InputActionReference clickAction;   // Pointer press

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

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        clickAction.action.started += OnClickStarted;
        clickAction.action.canceled += OnClickCanceled;

        clickAction.action.Enable();
        pointAction.action.Enable();
    }

    private void OnDisable()
    {
        clickAction.action.started -= OnClickStarted;
        clickAction.action.canceled -= OnClickCanceled;

        clickAction.action.Disable();
        pointAction.action.Disable();
    }

    private void Update()
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.DrawFloor)
            return;

        if (!isDragging) return;

        // 현재 마우스 위치 가져오기
        Vector3 rawCurrent = GetMouseWorldPos();

        // FloorPiece 스냅 + 10cm 단위 보정
        Vector3 current = SnapUtil.CleanPosition(rawCurrent, RoomManager.Instance.GetAllPieces());

        UpdatePreview(current);
    }

    // ===========================
    // 입력 시작 (드래그 시작)
    // ===========================
    private void OnClickStarted(InputAction.CallbackContext ctx)
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.DrawFloor)
            return;

        if (IsPointerOverUI())
            return;

        Vector3 rawStart = GetMouseWorldPos();

        // 시작점 보정: FloorPiece 경계 스냅 → 10cm 스냅
        dragStartPos = SnapUtil.CleanPosition(rawStart, RoomManager.Instance.GetAllPieces());

        StartPreview();
    }

    // ===========================
    // 입력 종료 (드래그 끝)
    // ===========================
    private void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        if (!isDragging) return;

        EndPreview();
    }

    // ---------------------------
    // 마우스/터치 위치 → 월드 좌표
    // ---------------------------
    private Vector3 GetMouseWorldPos()
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }

    // ---------------------------
    // 프리뷰 시작
    // ---------------------------
    private void StartPreview()
    {
        isDragging = true;
        previewObj = Instantiate(floorPreviewPrefab);

        widthLabel = Instantiate(dimensionLabelPrefab, uiCanvas.transform);
        heightLabel = Instantiate(dimensionLabelPrefab, uiCanvas.transform);
    }

    // ---------------------------
    // 드래그 중 프리뷰 업데이트
    // ---------------------------
    private void UpdatePreview(Vector3 currentPos)
    {
        Vector3 center = (dragStartPos + currentPos) / 2f;

        float width = Mathf.Abs(currentPos.x - dragStartPos.x);
        float depth = Mathf.Abs(currentPos.z - dragStartPos.z);

        // 최소 크기 10cm 보정
        if (width < SnapUtil.UNIT) width = SnapUtil.UNIT;
        if (depth < SnapUtil.UNIT) depth = SnapUtil.UNIT;

        previewObj.transform.position = center;
        previewObj.transform.localScale = new Vector3(width, 0.1f, depth);

        UpdateDimensionLabels(center, width, depth);
    }

    // ---------------------------
    // 드래그 종료 → 최종 바닥 생성
    // ---------------------------
    private void EndPreview()
    {
        isDragging = false;

        if (widthLabel != null) Destroy(widthLabel.gameObject);
        if (heightLabel != null) Destroy(heightLabel.gameObject);

        widthLabel = null;
        heightLabel = null;

        if (previewObj == null) return;

        Vector3 pos = previewObj.transform.position;
        Vector3 scale = previewObj.transform.localScale;

        Bounds candidateBounds = new Bounds(pos, scale);

        bool canPlace =
            RoomManager.Instance != null &&
            RoomManager.Instance.CanPlace(candidateBounds);

        Destroy(previewObj);
        previewObj = null;

        if (!canPlace)
        {
            Debug.Log("RoomManager: 기존 바닥과 이어지지 않아 생성하지 않음");
            return;
        }

        GameObject finalFloor = Instantiate(floorFinalPrefab);
        finalFloor.transform.position = pos;
        finalFloor.transform.localScale = scale;

        FloorPiece piece = finalFloor.GetComponent<FloorPiece>();
        if (piece == null)
            piece = finalFloor.AddComponent<FloorPiece>();

        RoomManager.Instance.RegisterPiece(piece);
    }

    // ---------------------------
    // UI 위 클릭/터치는 무시
    // ---------------------------
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Mouse.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.isInProgress &&
                    EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue()))
                    return true;
            }
        }

        return false;
    }

    // ---------------------------
    // 치수 Label 표시 계산
    // ---------------------------
    private void UpdateDimensionLabels(Vector3 center, float width, float depth)
    {
        if (widthLabel == null || heightLabel == null)
            return;

        float halfW = width * 0.5f;
        float halfD = depth * 0.5f;

        Vector3 topPos = new Vector3(center.x, 0.15f, center.z + halfD);
        Vector3 rightPos = new Vector3(center.x + halfW, 0.15f, center.z);

        Vector2 topS = cam.WorldToScreenPoint(topPos);
        Vector2 rightS = cam.WorldToScreenPoint(rightPos);

        widthLabel.SetLabel(topS, width);
        heightLabel.SetLabel(rightS, depth);
    }
}
