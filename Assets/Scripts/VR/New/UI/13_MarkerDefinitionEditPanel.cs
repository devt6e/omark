using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// MarkerDefinition 편집 패널
/// - 슬롯 탭으로 열림
/// - 즐겨찾기 상태는 Repository 소유
/// - 패널은 토글 UI 역할만 수행
/// </summary>
public class MarkerDefinitionEditPanel : MonoBehaviour
{
    // =========================
    // UI
    // =========================
    [Header("Inputs")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField descriptionInput;

    [Header("Buttons")]
    [SerializeField] private Button btnBack;
    [SerializeField] private Button btnEdit;

    [Header("Color Buttons (5)")]
    [SerializeField] private Button[] colorButtons;
    [SerializeField] private Color[] colors;

    // =========================
    // Refs
    // =========================
    [Header("Refs")]
    [SerializeField] private MarkerSlotSpawner slotSpawner;
    [SerializeField] private GameObject inventory;

    

    // =========================
    // Internal
    // =========================
    private string currentDefinitionId;
    private Color selectedColor;
    private int selectedColorIndex = -1;

    // =========================
    // Unity
    // =========================
    private void Awake()
    {
        // 색상 버튼 연결
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i;
            colorButtons[i].onClick.AddListener(() => SelectColor(index));
        }

        btnBack.onClick.AddListener(Close);
        btnEdit.onClick.AddListener(OnEdit);

        gameObject.SetActive(false);
    }

    // =========================
    // Open
    // =========================
    public void OpenForEdit(string definitionId)
    {
        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.GetById(definitionId);

        if (def == null)
        {
            Debug.LogError("[MarkerDefinitionEditPanel] Definition not found");
            return;
        }

        currentDefinitionId = definitionId;

        // UI 반영
        nameInput.text = def.DisplayName;
        descriptionInput.text = def.Description;

        // ⭐ 편집 패널 열리면 즐겨찾기 회전 중단
        // MarkerRotateAnimator.Instance.StopRotate();

        gameObject.SetActive(true);
    }

    // =========================
    // Button Logic
    // =========================
    private void OnEdit()
    {
        // Debug.Log("on click edit");
        string name = nameInput.text;
        if (string.IsNullOrWhiteSpace(name))
            name = "마커";

        MarkerDefinitionRepository.Instance.UpdateInfo(
            currentDefinitionId,
            name,
            descriptionInput.text,
            selectedColorIndex = selectedColorIndex < 0 ? 0 : selectedColorIndex,
            selectedColor
        );

        // 슬롯 UI 갱신
        slotSpawner.RefreshSlot(currentDefinitionId);
        Close();
    }

    // =========================
    // Close
    // =========================
    private void Close()
    {
        inventory.SetActive(true);
        gameObject.SetActive(false);
        currentDefinitionId = null;
        

        // ⭐ 패널 닫히면 즐겨찾기 회전 재개
        // MarkerRotateAnimator.Instance.StopRotate();
    }

    private void SelectColor(int index)
    {
        selectedColorIndex = index;
        selectedColor = colors[index];

        // 시각 피드백 (선택 강조)
        for (int i = 0; i < colorButtons.Length; i++)
        {
            colorButtons[i].transform.localScale =
                (i == index) ? Vector3.one * 1.2f : Vector3.one;
        }
    }   
}
