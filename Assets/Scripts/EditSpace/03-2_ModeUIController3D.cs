using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeUIController3D : MonoBehaviour
{
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

    private void UpdateUI(EditMode mode)
    {
        // 기본값
        SetAlpha(hlMove3D, inactiveAlpha);
        SetAlpha(hlPlaceFurniture, inactiveAlpha);
        SetAlpha(hlEditFurniture, inactiveAlpha);

        switch (mode)
        {
            case EditMode.MoveView3D:
                SetAlpha(hlMove3D, activeAlpha);
                modeText.text = "시점 이동";
                break;

            case EditMode.PlaceFurniture:
                SetAlpha(hlPlaceFurniture, activeAlpha);
                modeText.text = "가구 배치";
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
