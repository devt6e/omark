// using UnityEngine;
// using UnityEngine.UI;

// public class UIScrollGate : MonoBehaviour
// {
//     [SerializeField] private ScrollRect scrollRect;

//     private void Awake()
//     {
//         if (scrollRect == null)
//             scrollRect = GetComponent<ScrollRect>();
//     }

//     public void Lock()
//     {
//         if (scrollRect != null)
//             scrollRect.enabled = false;
//     }

//     public void Unlock()
//     {
//         if (scrollRect != null)
//             scrollRect.enabled = true;
//     }
// }
// // 