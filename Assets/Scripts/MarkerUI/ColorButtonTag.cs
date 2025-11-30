using UnityEngine;

public class ColorButtonTag : MonoBehaviour
{
    // Unity Editor에서 이 버튼이 대표하는 색상 코드 (예: "#FF0000")를 직접 입력합니다.
    public string buttonColorCode;

    // 시각적 강조를 위해 버튼 위에 띄울 오브젝트 (예: 테두리, 체크마크 이미지)
    public GameObject selectionHighlight;

    void Start()
    {
        // 강조 오브젝트는 시작 시 숨겨둡니다.
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }
    }
}