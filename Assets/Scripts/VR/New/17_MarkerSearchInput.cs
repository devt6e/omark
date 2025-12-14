// using UnityEngine;
// using TMPro;

// /// <summary>
// /// 마커 검색 InputField 전용 브리지
// /// </summary>
// public class MarkerSearchInput : MonoBehaviour
// {
//     [SerializeField] private TMP_InputField inputField;

//     private void Awake()
//     {
//         if (inputField == null)
//             inputField = GetComponent<TMP_InputField>();

//         inputField.onValueChanged.AddListener(OnValueChanged);
//     }

//     private void OnDestroy()
//     {
//         if (inputField != null)
//             inputField.onValueChanged.RemoveListener(OnValueChanged);
//     }

//     private void OnValueChanged(string text)
//     {
//         MarkerFilterController.Instance.SetSearchText(text);
//     }
// }
