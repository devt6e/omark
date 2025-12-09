// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class ModeUIController : MonoBehaviour
// {
//     [Header("Buttons")]
//     public Button moveButton;
//     public Button drawRoomButton;
//     public Button furnitureButton;

//     [Header("Highlight Images (Child of Each Button)")]
//     public Image moveHighlight;
//     public Image drawRoomHighlight;
//     public Image furnitureHighlight;

//     [Header("Mode Text")]
//     public TMP_Text modeText;

//     private void Start()
//     {
//         moveButton.onClick.AddListener(() => OnModeClicked(EditMode.MoveView));
//         drawRoomButton.onClick.AddListener(() => OnModeClicked(EditMode.DrawFloor));
//         furnitureButton.onClick.AddListener(() => OnModeClicked(EditMode.PlaceFurniture));

//         UpdateUI(EditMode.MoveView);
//     }

//     private void OnModeClicked(EditMode mode)
//     {
//         EditorModeManager.Instance.SetMode(mode);
//         UpdateUI(mode);
//     }

//     private void UpdateUI(EditMode mode)
//     {
//         // 알파 값 계산
//         float alphaSelected = 255f / 255f;   // 1.0
//         float alphaNormal = 150f / 255f;     // 약 0.588

//         // 기본값(모두 150)
//         SetAlpha(moveHighlight, alphaNormal);
//         SetAlpha(drawRoomHighlight, alphaNormal);
//         SetAlpha(furnitureHighlight, alphaNormal);

//         // 선택된 버튼만 255
//         switch (mode)
//         {
//             case EditMode.MoveView:
//                 SetAlpha(moveHighlight, alphaSelected);
//                 modeText.text = "기본 모드";
//                 break;

//             case EditMode.DrawFloor:
//                 SetAlpha(drawRoomHighlight, alphaSelected);
//                 modeText.text = "방 그리기";
//                 break;

//             case EditMode.PlaceFurniture:
//                 SetAlpha(furnitureHighlight, alphaSelected);
//                 modeText.text = "가구 배치";
//                 break;
//         }
//     }

//     private void SetAlpha(Image img, float alpha)
//     {
//         if (img == null) return;

//         Color c = img.color;
//         c.a = alpha;
//         img.color = c;
//     }
// }
