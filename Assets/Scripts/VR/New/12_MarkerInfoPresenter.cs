using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 마커 정보 패널 표시 전용 Presenter
/// - MarkerDefinition 기반 정보 표시
/// - Show / Hide 책임
/// - 입력 판단 없음
/// </summary>
public class MarkerInfoPresenter : MonoBehaviour
{
    // =========================
    // UI Refs
    // =========================
    [Header("Ref")]
    [SerializeField] private GameObject inventory;
    [SerializeField] private MarkerDefinitionEditPanel editInfo;
    [SerializeField] private MarkerSlotSpawner slotSpawner;

    [Header("Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Favorite UI")]
    [SerializeField] private Image favoriteIcon;
    [SerializeField] private Sprite favoriteOnSprite;
    [SerializeField] private Sprite favoriteOffSprite;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;        // 마커 이름
    [SerializeField] private TMP_Text descriptionText;  // 마커 상세 정보

    [Header("Buttons")]
    [SerializeField] private Button btnClose;   // 되돌아가기(닫기)
    [SerializeField] private Button btnDelete;  // 삭제
    [SerializeField] private Button btnEdit;    // 편집
    [SerializeField] private Button btnFavorite; // 즐겨찾기 (선택)

    // =========================
    // Internal State
    // =========================
    private string currentDefinitionId;
    public bool IsVisible => panelRoot.activeSelf;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (btnClose != null)
            btnClose.onClick.AddListener(Hide);
    }

    // =========================
    // Public API
    // =========================

    /// <summary>
    /// 마커 정보 표시
    /// - 같은 마커를 다시 요청하면 토글로 닫힘
    /// </summary>
    public void Show(string definitionId)
    {
        panelRoot.SetActive(true);
        if (inventory != null)
            inventory.SetActive(false);
        if (string.IsNullOrEmpty(definitionId))
            return;

        // 같은 마커 다시 탭 → 닫기
        if (IsVisible && currentDefinitionId == definitionId)
        {
            inventory.SetActive(true);
            Hide();
            return;
        }

        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.GetById(definitionId);
    
        if (def == null)
        {
            Debug.LogError("[MarkerInfoPresenter] Definition not found");
            return;
        }
        UpdateFavoriteIcon(def.IsFavorite);
        currentDefinitionId = definitionId;
        ApplyDefinition(def);

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
        currentDefinitionId = null;
    }

    // =========================
    // Internal
    // =========================
    private void ApplyDefinition(MarkerDefinition def)
    {
        if (titleText != null)
            titleText.text = string.IsNullOrEmpty(def.DisplayName)
                ? "이름 없음"
                : def.DisplayName;

        if (descriptionText != null)
            descriptionText.text = string.IsNullOrEmpty(def.Description)
                ? "비어있음..."
                : def.Description;

        // 버튼 로직은 여기서 연결만 해둔다 (정책은 나중에)
        if (btnDelete != null)
        {
            btnDelete.onClick.RemoveAllListeners();
            btnDelete.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(currentDefinitionId))
                    return;

                ConfirmPopup.Instance.Open(() =>
                    {
                        bool removed = MarkerDefinitionRepository.Instance.Remove(currentDefinitionId);
                        if (!removed)
                            return;

                        // 1. 슬롯 제거
                        slotSpawner.RemoveSlot(currentDefinitionId);

                        // 2. 배치된 마커 제거
                        MarkerInstanceRegistry.Instance.RemoveAllByDefinition(currentDefinitionId);

                        // 3. InfoPanel 닫기
                        inventory.SetActive(true);
                        Hide();
                    }
                );
            });
        }

        if (btnEdit != null)
        {
            btnEdit.onClick.RemoveAllListeners();
            btnEdit.onClick.AddListener(() =>
            {
                Debug.Log($"[MarkerInfo] Edit {def.DefinitionId}");
                if (editInfo != null)
                {
                    editInfo.OpenForEdit(currentDefinitionId);
                    Hide();
                }



            });
        }

        if (btnFavorite != null)
        {
            btnFavorite.onClick.RemoveAllListeners();
            btnFavorite.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(currentDefinitionId))
                    return;

                var def = MarkerDefinitionRepository.Instance
                    .GetById(currentDefinitionId);

                if (def == null)
                    return;

                def.ToggleFavorite();

                // 버튼 시각 갱신 (선택)
                UpdateFavoriteIcon(def.IsFavorite);
            });
        }

        if (btnClose != null)
        {
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() =>
            {
                Debug.Log($"[MarkerInfo] Closer {def.DefinitionId}");
                inventory.SetActive(true);
                Hide();
            });
        }
    }

    private void UpdateFavoriteIcon(bool isFavorite)
    {
        if (favoriteIcon == null)
            return;

        favoriteIcon.sprite =
            isFavorite ? favoriteOnSprite : favoriteOffSprite;
    }
}
