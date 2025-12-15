using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MarkerCreatePanel : MonoBehaviour
{
    [Header("Color Buttons (5)")]
    [SerializeField] private Button[] colorButtons;
    [SerializeField] private Color[] colors;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField descriptionInput;

    [Header("Buttons")]
    [SerializeField] private Button btnCreate;
    [SerializeField] private Button btnClose;

    [Header("Refs")]
    [SerializeField] private MarkerSlotSpawner slotSpawner;
    [SerializeField] private GameObject Inventory;
    
    private Color selectedColor;
    private int selectedColorIndex = -1;

    private void Awake()
    {
        // 색상 버튼 연결
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i;
            colorButtons[i].onClick.AddListener(() => SelectColor(index));
        }
        btnCreate.onClick.AddListener(OnCreate);
        btnClose.onClick.AddListener(Close);
        gameObject.SetActive(false);
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

    private void OnCreate()
    {
        // Debug.Log($"name : {nameInput.text}\ncolor : {selectedColor}\ncolorIndex : {selectedColorIndex}\ndes : {descriptionInput.text} ");
        string name = nameInput.text;
        if (string.IsNullOrWhiteSpace(name))
            name = "마커";
        if (selectedColorIndex < 0)
            selectedColorIndex = 4;

        // 1. Definition 생성
        MarkerDefinition def =
            MarkerDefinitionRepository.Instance.Create(
                name,
                selectedColor,
                selectedColorIndex,
                descriptionInput.text
            );

        // 2. 슬롯 자동 생성
        MarkerDefinitionSlot slot =
            slotSpawner.SpawnSlot(def.DefinitionId);

        // 3. 즉시 배치 시작
        slot.BeginPlacementFromCode();

        // 정리
        nameInput.text = string.Empty;
        descriptionInput.text = string.Empty;
        Close();
    }

    private void Close()
    {
        Inventory.SetActive(true);
        gameObject.SetActive(false);
    }
}
