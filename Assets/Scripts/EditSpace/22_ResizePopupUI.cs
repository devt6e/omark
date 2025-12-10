using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResizePopupUI : MonoBehaviour
{
    public static ResizePopupUI Instance;

    [Header("UI References")]
    public GameObject popupRoot;

    public TMP_InputField inputWidthCm;
    public TMP_InputField inputHeightCm;

    public TMP_Text placeholderWidth;
    public TMP_Text placeholderHeight;

    public Button btnConfirm;
    public Button btnCancel;

    // 현재 리사이즈 중인 FloorPiece
    private FloorPiece targetPiece;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        popupRoot.SetActive(false);

        inputWidthCm.onEndEdit.AddListener(OnWidthEndEdit);
        inputHeightCm.onEndEdit.AddListener(OnHeightEndEdit);

        // 숫자 외 입력 방지
        inputWidthCm.onValidateInput += ValidateNumeric;
        inputHeightCm.onValidateInput += ValidateNumeric;

        btnConfirm.onClick.AddListener(OnClickConfirm);
        btnCancel.onClick.AddListener(OnClickCancel);
    }

    private void OnWidthEndEdit(string value)
    {
        if (!TryNormalizeCm(value, out int rounded)) return;

        inputWidthCm.SetTextWithoutNotify(rounded.ToString());
    }

    private void OnHeightEndEdit(string value)
    {
        if (!TryNormalizeCm(value, out int rounded)) return;

        inputHeightCm.SetTextWithoutNotify(rounded.ToString());
    }

    // ============================================================
    // 팝업 열기
    // ============================================================
    public void Show(FloorPiece piece)
    {
        if (piece == null) return;

        targetPiece = piece;

        // 현재 바닥 크기를 cm 단위로 가져오기
        Bounds b = piece.GetBounds();
        float widthCm  = b.size.x * 100f;
        float heightCm = b.size.z * 100f;

        int roundedWidth  = Mathf.RoundToInt(widthCm);
        int roundedHeight = Mathf.RoundToInt(heightCm);

        // TextField는 비워두고 Placeholder만 표시하도록 처리
        inputWidthCm.SetTextWithoutNotify("");
        inputHeightCm.SetTextWithoutNotify("");

        placeholderWidth.text  = roundedWidth  + " cm";
        placeholderHeight.text = roundedHeight + " cm";

        popupRoot.SetActive(true);
    }

    public void Hide()
    {
        popupRoot.SetActive(false);
        targetPiece = null;
    }

    // ============================================================
    // onValueChanged → 실시간 10cm 단위 반올림
    // ============================================================
    private void OnWidthInputChanged(string value)
    {
        if (!TryNormalizeCm(value, out int rounded)) return;
        inputWidthCm.SetTextWithoutNotify(rounded.ToString());
    }

    private void OnHeightInputChanged(string value)
    {
        if (!TryNormalizeCm(value, out int rounded)) return;
        inputHeightCm.SetTextWithoutNotify(rounded.ToString());
    }

    private bool TryNormalizeCm(string value, out int rounded)
    {
        rounded = 0;

        if (string.IsNullOrEmpty(value)) return false;
        if (!int.TryParse(value, out int v)) return false;

        // 10cm 단위 반올림
        rounded = Mathf.RoundToInt(v / 10f) * 10;
        return true;
    }

    // ============================================================
    // 숫자 외 입력 방지
    // ============================================================
    private char ValidateNumeric(string text, int charIndex, char addedChar)
    {
        // 숫자 아니면 무시
        if (addedChar < '0' || addedChar > '9')
            return '\0';  // 입력 무효화
        return addedChar;
    }

    // ============================================================
    // 확인 버튼 → ResizeManager 호출
    // ============================================================
    private void OnClickConfirm()
    {
        if (targetPiece == null) return;

        Bounds b = targetPiece.GetBounds();
        float currentWidthCm  = b.size.x * 100f;
        float currentHeightCm = b.size.z * 100f;

        int widthCm;
        int heightCm;

        // 빈칸 입력 → 기존 크기 유지
        if (string.IsNullOrEmpty(inputWidthCm.text))
            widthCm = Mathf.RoundToInt(currentWidthCm);
        else
            widthCm = int.Parse(inputWidthCm.text);

        if (string.IsNullOrEmpty(inputHeightCm.text))
            heightCm = Mathf.RoundToInt(currentHeightCm);
        else
            heightCm = int.Parse(inputHeightCm.text);

        // 최소값 10cm 보정
        widthCm  = Mathf.Max(10, widthCm);
        heightCm = Mathf.Max(10, heightCm);

        // ResizeManager 호출
        ResizeManager.Instance.ApplyResize(targetPiece, widthCm, heightCm);

        Hide();
    }

    // ============================================================
    // 취소 버튼
    // ============================================================
    private void OnClickCancel()
    {
        Hide();
    }
}
