using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SizeUIController : MonoBehaviour
{
    public static SizeUIController Instance;

    [Header("UI References")]
    public RectTransform uiWidth;      // 우측 중앙
    public RectTransform uiHeight;     // 하단 중앙

    public TMP_InputField inputWidth;
    public TMP_InputField inputHeight;

    public Button btnWidthOK;
    public Button btnWidthCancel;
    public Button btnHeightOK;
    public Button btnHeightCancel;

    private FloorPiece target;

    private Canvas mainCanvas;
    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        mainCanvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;

        Hide();

        // 버튼 리스너 등록
        btnWidthOK.onClick.AddListener(ConfirmWidth);
        btnWidthCancel.onClick.AddListener(CancelWidth);
        btnHeightOK.onClick.AddListener(ConfirmHeight);
        btnHeightCancel.onClick.AddListener(CancelHeight);
    }

    // ===============================
    // 표시 / 숨기기
    // ===============================
    public void Show(FloorPiece piece)
    {
        target = piece;
        gameObject.SetActive(true);

        inputWidth.text = piece.transform.localScale.x.ToString("F2");
        inputHeight.text = piece.transform.localScale.z.ToString("F2");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        target = null;
    }

    // ===============================
    // 실제 값 적용
    // ===============================
    public void ConfirmWidth()
    {
        if (target == null) return;

        if (float.TryParse(inputWidth.text, out float v))
            target.ApplyWidth(v);
    }

    public void CancelWidth()
    {
        if (target == null) return;

        inputWidth.text = target.transform.localScale.x.ToString("F2");
    }

    public void ConfirmHeight()
    {
        if (target == null) return;

        if (float.TryParse(inputHeight.text, out float v))
            target.ApplyHeight(v);
    }

    public void CancelHeight()
    {
        if (target == null) return;

        inputHeight.text = target.transform.localScale.z.ToString("F2");
    }

    // ===============================
    // UI 위치 갱신
    // ===============================
    public void UpdateUIPositions(FloorPiece p)
    {
        if (mainCanvas == null || mainCamera == null) return;

        Bounds b = p.GetBounds();

        Vector3 right = new Vector3(b.max.x, 0, b.center.z);
        Vector3 bottom = new Vector3(b.center.x, 0, b.min.z);

        UpdateUIPosition(uiWidth, right);
        UpdateUIPosition(uiHeight, bottom);
    }

    private void UpdateUIPosition(RectTransform ui, Vector3 worldPos)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = mainCanvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPos
        );

        ui.anchoredPosition = localPos;
    }
}
