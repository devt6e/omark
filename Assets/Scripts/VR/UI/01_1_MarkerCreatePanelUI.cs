// using UnityEngine;
// using TMPro;

// public class MarkerCreatePanelUI : MonoBehaviour
// {
//     [Header("Root")]
//     [SerializeField] private GameObject panelRoot;
//     [SerializeField] private GameObject inventoryRoot;

//     [Header("Inputs")]
//     [SerializeField] private TMP_InputField inputName;
//     [SerializeField] private TMP_InputField inputDescription;

//     [Header("Color")]
//     [SerializeField] private MarkerColorSelector colorSelector;

//     [Header("Inventory")]
//     [SerializeField] private MarkerInventoryController inventoryController;

//     private void Awake()
//     {
//         panelRoot.SetActive(false);
//     }

//     public void Open()
//     {
//         ClearInputs();
//         panelRoot.SetActive(true);
//         if (inventoryRoot != null)
//             inventoryRoot.SetActive(false);
//     }

//     public void Close()
//     {
//         panelRoot.SetActive(false);
//         if (inventoryRoot != null)
//             inventoryRoot.SetActive(true);
//     }

//     public void OnClickConfirm()
//     {
//         if (string.IsNullOrEmpty(inputName.text))
//             return;

//         T6MarkerItemData data = new T6MarkerItemData(
//             inputName.text,
//             inputDescription.text,
//             colorSelector.CurrentColor
//         );

//         inventoryController.AddMarkerLaunchPad(data);
//         Close();
//     }

//     private void ClearInputs()
//     {
//         inputName.text = string.Empty;
//         inputDescription.text = string.Empty;
//     }
// }
