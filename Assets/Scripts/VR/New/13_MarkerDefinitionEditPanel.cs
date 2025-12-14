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
    [SerializeField] private Button btnDelete;

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

    // =========================
    // Unity
    // =========================
    private void Awake()
    {
        btnBack.onClick.AddListener(Close);
        btnEdit.onClick.AddListener(OnEdit);
        btnDelete.onClick.AddListener(OnDelete);

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
        string name = nameInput.text;
        if (string.IsNullOrWhiteSpace(name))
            name = "마커";

        MarkerDefinitionRepository.Instance.UpdateInfo(
            currentDefinitionId,
            name,
            descriptionInput.text
        );

        // 슬롯 UI 갱신
        slotSpawner.RefreshSlot(currentDefinitionId);
    }

    private void OnDelete()
    {
        // Repository에서 정의 삭제
        MarkerDefinitionRepository.Instance.Delete(currentDefinitionId);

        // 슬롯 제거
        slotSpawner.RemoveSlot(currentDefinitionId);

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
}
