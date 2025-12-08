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

    private GameObject previewObj;
    private bool isDragging = false;
    private Vector3 dragStartPos;

    private const float GRID = 0.5f; // 500mm

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
        // 방 그리기 모드가 아니면 완전히 무시
        if (EditorModeManager.Instance.CurrentMode != EditMode.DrawFloor)
            return;

        if (!isDragging) return;

        Vector3 current = SnapToGrid(GetMouseWorldPos());
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

        dragStartPos = SnapToGrid(GetMouseWorldPos());
        StartPreview();
    }

    // ===========================
    // 입력 종료 (드래그 끝)
    // ===========================
    private void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        if (EditorModeManager.Instance.CurrentMode != EditMode.DrawFloor)
            return;

        if (isDragging)
            EndPreview();
    }

    // ---------------------------
    // 마우스/터치 위치 → 월드 좌표
    // ---------------------------
    private Vector3 GetMouseWorldPos()
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        // y=0 평면에 투영
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }

    // ---------------------------
    // 그리드(500mm) 스냅
    // ---------------------------
    private Vector3 SnapToGrid(Vector3 pos)
    {
        pos.x = Mathf.Round(pos.x / GRID) * GRID;
        pos.z = Mathf.Round(pos.z / GRID) * GRID;
        return pos;
    }

    // ---------------------------
    // 프리뷰 시작
    // ---------------------------
    private void StartPreview()
    {
        isDragging = true;
        previewObj = Instantiate(floorPreviewPrefab);
    }

    // ---------------------------
    // 드래그 중 프리뷰 업데이트
    // ---------------------------
    private void UpdatePreview(Vector3 currentPos)
    {
        Vector3 center = (dragStartPos + currentPos) / 2f;

        float width = Mathf.Abs(currentPos.x - dragStartPos.x);
        float depth = Mathf.Abs(currentPos.z - dragStartPos.z);

        // 최소 크기 보호 (실수로 너무 작은 드래그)
        if (width < GRID) width = GRID;
        if (depth < GRID) depth = GRID;

        previewObj.transform.position = center;
        previewObj.transform.localScale = new Vector3(width, 0.1f, depth);
    }

    // ---------------------------
    // 드래그 종료 → 최종 바닥 생성 시도
    // ---------------------------
    private void EndPreview()
    {
        isDragging = false;

        if (previewObj == null) return;

        Vector3 pos = previewObj.transform.position;
        Vector3 scale = previewObj.transform.localScale;

        // 미리보기 Bounds 계산
        Bounds candidateBounds = new Bounds(pos, scale);

        // RoomManager에게 이 위치에 생성 가능한지 물어봄
        bool canPlace = RoomManager.Instance != null &&
                        RoomManager.Instance.CanPlace(candidateBounds);

        Destroy(previewObj);
        previewObj = null;

        if (!canPlace)
        {
            Debug.Log("RoomManager: 기존 바닥과 이어지지 않아서 바닥을 생성하지 않습니다.");
            return;
        }

        // 생성 가능하면 실제 FloorPiece 생성
        GameObject finalFloor = Instantiate(floorFinalPrefab);
        finalFloor.transform.position = pos;
        finalFloor.transform.localScale = scale;

        // FloorPiece 컴포넌트 가져오기
        FloorPiece piece = finalFloor.GetComponent<FloorPiece>();
        if (piece == null)
        {
            piece = finalFloor.AddComponent<FloorPiece>();
        }

        // RoomManager에 등록
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.RegisterPiece(piece);
        }
    }

    // ---------------------------
    // UI 위 클릭/터치는 무시
    // ---------------------------
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // 마우스
        if (Mouse.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        // 터치
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.isInProgress)
                {
                    int id = touch.touchId.ReadValue();
                    if (EventSystem.current.IsPointerOverGameObject(id))
                        return true;
                }
            }
        }

        return false;
    }
}
