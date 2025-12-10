using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeUIController2D : MonoBehaviour
{
    public static ModeUIController2D Instance {get; private set;}

    [Header("Buttons")]
    public Button btnMove;
    public Button btnDrawFloor;
    public Button btnEditFloor;

    [Header("Highlight Images")]
    public Image hlMove;
    public Image hlDrawFloor;
    public Image hlEditFloor;

    [Header("Mode Text")]
    public TMP_Text modeText;

    [Header("Alpha Settings")]
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.6f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        btnMove.onClick.AddListener(() => OnModeClicked(EditMode.MoveView2D));
        btnDrawFloor.onClick.AddListener(() => OnModeClicked(EditMode.DrawFloor));
        btnEditFloor.onClick.AddListener(() => OnModeClicked(EditMode.EditFloor));

        UpdateUI(EditMode.MoveView2D);
    }

    private void OnModeClicked(EditMode mode)
    {
        EditorModeManager.Instance.SetMode(mode);
        UpdateUI(mode);
    }

    public void UpdateUI(EditMode mode)
    {
        // 모든 버튼 기본 세팅
        SetAlpha(hlMove, inactiveAlpha);
        SetAlpha(hlDrawFloor, inactiveAlpha);
        SetAlpha(hlEditFloor, inactiveAlpha);

        switch (mode)
        {
            case EditMode.MoveView2D:
                SetAlpha(hlMove, activeAlpha);
                modeText.text = "기본 모드";
                break;

            case EditMode.DrawFloor:
                SetAlpha(hlDrawFloor, activeAlpha);
                modeText.text = "바닥 그리기";
                break;

            case EditMode.EditFloor:
                SetAlpha(hlEditFloor, activeAlpha);
                modeText.text = "바닥 편집";
                break;
        }
    }

    private void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
