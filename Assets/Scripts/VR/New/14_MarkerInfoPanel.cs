using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 마커 정보 표시 전용 패널
/// - 슬롯 탭 시 열림
/// - 읽기 전용 (이름 / 설명)
/// - 편집 버튼을 통해 EditPanel로 진입
/// </summary>
public class MarkerInfoPanel : MonoBehaviour
{
    // =========================
    // UI
    // =========================
    [Header("Texts")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Buttons")]
    [SerializeField] private Button btnEdit;
    [SerializeField] private Button btnClose;

    [Header("Favorite")]
    [SerializeField] private Button btnFavorite;
    [SerializeField] private Image favoriteIcon;
    [SerializeField] private Sprite favoriteOn;
    [SerializeField] private Sprite favoriteOff;

    // =========================
    // Refs
    // =========================
    [Header("Refs")]
    [SerializeField] private MarkerDefinitionEditPanel editPanel;
    [SerializeField] private GameObject inventory;

    // =========================
    // Internal
    // =========================
    private string currentDefinitionId;

    // =========================
    // Unity
    // =========================
    private void Awake()
    {
        btnEdit.onClick.AddListener(OnEdit);
        btnClose.onClick.AddListener(Close);
        btnFavorite.onClick.AddListener(OnToggleFavorite);

        gameObject.SetActive(false);
    }

    // =========================
    // Open / Close
    // =========================

    /// <summary>
    /// 마커 정보 패널 열기
    /// (슬롯 탭 시 호출)
    /// </summary>
    public void Open(string definitionId)
    {   
        gameObject.SetActive(true);
        inventory.SetActive(false);
        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.GetById(definitionId);

        if (def == null)
        {
            Debug.LogError("[MarkerInfoPanel] Definition not found");
            return;
        }

        currentDefinitionId = definitionId;

        nameText.text = def.DisplayName;
        descriptionText.text = def.Description;

        RefreshFavoriteIcon(def.IsFavorite);

        // gameObject.SetActive(true);
    }

    private void Close()
    {
        currentDefinitionId = null;
        inventory.SetActive(true);
        gameObject.SetActive(false);
    }

    // =========================
    // Buttons
    // =========================

    private void OnEdit()
    {
        if (editPanel == null || string.IsNullOrEmpty(currentDefinitionId))
            return;

        gameObject.SetActive(false);
        editPanel.OpenForEdit(currentDefinitionId);
    }


    private void OnToggleFavorite()
    {
        if (string.IsNullOrEmpty(currentDefinitionId))
            return;

        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.GetById(currentDefinitionId);

        if (def == null)
            return;

        bool newValue = !def.IsFavorite;

        MarkerDefinitionRepository.Instance.SetFavorite(
            currentDefinitionId,
            newValue
        );

        RefreshFavoriteIcon(newValue);
    }

    private void RefreshFavoriteIcon(bool isFavorite)
    {
        if (favoriteIcon != null)
            favoriteIcon.sprite = isFavorite ? favoriteOn : favoriteOff;
    }

 
}
