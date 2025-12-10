using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeUIController3D : MonoBehaviour
{
    public static ModeUIController3D Instance { get; private set; }

    [Header("Buttons")]
    public Button btnMove3D;
    public Button btnPlaceFurniture;
    public Button btnEditFurniture;

    [Header("Highlight Images")]
    public Image hlMove3D;
    public Image hlPlaceFurniture;
    public Image hlEditFurniture;

    [Header("Mode Text")]
    public TMP_Text modeText;

    [Header("Alpha Settings")]
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.6f;

    [Header("Furniture UI")]
    public GameObject furnitureCreateButton;  
    // ← "가구 생성하기" 버튼 오브젝트 연결해두기

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        btnMove3D.onClick.AddListener(() => OnModeClicked(EditMode.MoveView3D));
        btnPlaceFurniture.onClick.AddListener(() => OnModeClicked(EditMode.PlaceFurniture));
        btnEditFurniture.onClick.AddListener(() => OnModeClicked(EditMode.EditFurniture));

        UpdateUI(EditMode.MoveView3D);
    }

    private void OnModeClicked(EditMode mode)
    {
        EditorModeManager.Instance.SetMode(mode);
        UpdateUI(mode);
    }

    public void UpdateUI(EditMode mode)
    {
        // 기본값 초기화
        SetAlpha(hlMove3D, inactiveAlpha);
        SetAlpha(hlPlaceFurniture, inactiveAlpha);
        SetAlpha(hlEditFurniture, inactiveAlpha);

        // 모든 모드에서 기본적으로 숨겨두고, 필요할 때만 켜준다.
        if (furnitureCreateButton != null)
            furnitureCreateButton.SetActive(false);

        switch (mode)
        {
            case EditMode.MoveView3D:
                SetAlpha(hlMove3D, activeAlpha);
                modeText.text = "시점 이동";
                break;

            case EditMode.PlaceFurniture:
                SetAlpha(hlPlaceFurniture, activeAlpha);
                modeText.text = "가구 배치";

                // 가구 배치 모드일 때만 버튼 표시
                if (furnitureCreateButton != null)
                    furnitureCreateButton.SetActive(true);

                break;

            case EditMode.EditFurniture:
                SetAlpha(hlEditFurniture, activeAlpha);
                modeText.text = "가구 편집";
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
