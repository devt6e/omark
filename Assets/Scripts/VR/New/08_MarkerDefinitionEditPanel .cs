using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// MarkerDefinition 생성 / 편집 통합 패널
/// - UI는 동일
/// - 모드(Create / Edit)에 따라 동작만 분기
/// </summary>
public class MarkerDefinitionEditPanel : MonoBehaviour
{
    // =========================
    // Mode
    // =========================
    private enum PanelMode
    {
        Create,
        Edit
    }

    private PanelMode mode;
    private string editingDefinitionId;

    // =========================
    // UI
    // =========================
    [Header("Color Buttons (5)")]
    [SerializeField] private Button[] colorButtons;
    [SerializeField] private Color[] colors;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField descriptionInput;

    [Header("Buttons")]
    [SerializeField] private Button btnConfirm; // 생성 / 확인
    [SerializeField] private Button btnClose;

    [Header("Optional Text")]
    [SerializeField] private TMP_Text titleText; // "마커 생성" / "마커 편집"

    // =========================
    // Refs
    // =========================
    [Header("Refs")]
    [SerializeField] private MarkerSlotSpawner slotSpawner;
    [SerializeField] private GameObject inventoryRoot;

    // =========================
    // Internal
    // =========================
    private Color selectedColor;
    private int selectedColorIndex = -1;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        // 색상 버튼 연결
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i;
            colorButtons[i].onClick.AddListener(() => SelectColor(index));
        }

        btnConfirm.onClick.AddListener(OnConfirm);
        btnClose.onClick.AddListener(Close);
    }

    // =========================
    // Open API
    // =========================

    /// <summary>
    /// 새 MarkerDefinition 생성 모드로 열기
    /// </summary>
    public void OpenForCreate()
    {
        mode = PanelMode.Create;
        editingDefinitionId = null;

        ClearInputs();
        SetConfirmText("마커 생성");

        inventoryRoot.SetActive(false);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 기존 MarkerDefinition 편집 모드로 열기
    /// </summary>
    public void OpenForEdit(string definitionId)
    {
        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.GetById(definitionId);

        if (def == null)
        {
            Debug.LogError("[MarkerDefinitionEditPanel] Definition not found");
            return;
        }

        mode = PanelMode.Edit;
        editingDefinitionId = definitionId;

        ApplyDefinition(def);
        SetConfirmText("마커 편집");

        inventoryRoot.SetActive(false);
        gameObject.SetActive(true);
    }

    // =========================
    // UI Handling
    // =========================
    private void SelectColor(int index)
    {
        selectedColorIndex = index;
        selectedColor = colors[index];

        // 시각 피드백
        for (int i = 0; i < colorButtons.Length; i++)
        {
            colorButtons[i].transform.localScale =
                (i == index) ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    private void ClearInputs()
    {
        nameInput.text = string.Empty;
        descriptionInput.text = string.Empty;

        selectedColorIndex = -1;
        selectedColor = Color.white;

        for (int i = 0; i < colorButtons.Length; i++)
        {
            colorButtons[i].transform.localScale = Vector3.one;
        }
    }

    private void ApplyDefinition(MarkerDefinition def)
    {
        nameInput.text = def.DisplayName;
        descriptionInput.text = def.Description;

        SelectColor(def.ColorIndex);
    }

    private void SetConfirmText(string text)
    {
        if (titleText != null)
            titleText.text = text;
    }

    // =========================
    // Confirm
    // =========================
    private void OnConfirm()
    {
        string name = nameInput.text;
        if (string.IsNullOrWhiteSpace(name))
            name = "마커";

        if (selectedColorIndex < 0)
            selectedColorIndex = colors.Length - 1;

        if (mode == PanelMode.Create)
        {
            CreateDefinition(name);
        }
        else
        {
            UpdateDefinition(name);
        }

        Close();
    }

    private void CreateDefinition(string name)
    {
        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.Create(
                name,
                colors[selectedColorIndex],
                selectedColorIndex,
                descriptionInput.text
            );

        // 슬롯 생성
        MarkerDefinitionSlot slot =
            slotSpawner.SpawnSlot(def.DefinitionId);

        // 즉시 배치 시작
        slot.BeginPlacementFromCode();
    }

    private void UpdateDefinition(string name)
    {
        MarkerDefinitionRepository.Instance.UpdateInfo(
            editingDefinitionId,
            name,
            colors[selectedColorIndex],
            selectedColorIndex,
            descriptionInput.text
        );

        // 슬롯 UI 갱신 (있다면)
        slotSpawner.RefreshSlot(editingDefinitionId);
    }

    // =========================
    // Close
    // =========================
    private void Close()
    {
        if (mode == PanelMode.Create)
        {
            ClearInputs(); // ⭐ 생성 모드에서만 초기화
        }

        inventoryRoot.SetActive(true);
        gameObject.SetActive(false);
    }
}
