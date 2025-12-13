// using UnityEngine;

// public class MarkerSelectable : MonoBehaviour
// {
//     [Header("Selection State")]
//     public bool IsSelected { get; private set; }

//     [Header("Visual")]
//     [SerializeField] private Transform visualRoot;   // 실제 메시 루트
//     [SerializeField] private GameObject highlightObj; // 하이라이트 오브젝트

//     [Header("Scale")]
//     [SerializeField] private float selectedScaleMultiplier = 1.1f;

//     private Vector3 originalScale;

//     private void Awake()
//     {
//         if (visualRoot == null)
//             visualRoot = transform;

//         originalScale = visualRoot.localScale;

//         if (highlightObj != null)
//             highlightObj.SetActive(false);
//     }

//     // ----------------------------
//     // 선택 / 해제 API
//     // ----------------------------
//     public void Select()
//     {
//         if (IsSelected) return;

//         IsSelected = true;
//         ApplySelectedVisual();
//     }

//     public void Deselect()
//     {
//         if (!IsSelected) return;

//         IsSelected = false;
//         ApplyDeselectedVisual();
//     }

//     // ----------------------------
//     // 시각 처리
//     // ----------------------------
//     private void ApplySelectedVisual()
//     {
//         visualRoot.localScale = originalScale * selectedScaleMultiplier;

//         if (highlightObj != null)
//             highlightObj.SetActive(true);
//     }

//     private void ApplyDeselectedVisual()
//     {
//         visualRoot.localScale = originalScale;

//         if (highlightObj != null)
//             highlightObj.SetActive(false);
//     }
// }
