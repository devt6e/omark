using UnityEngine;
using UnityEngine.UI;

public class FurnitureCopyButton : MonoBehaviour
{
    public static FurnitureCopyButton Instance { get; private set; }

    public Button copyButton;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

        copyButton.onClick.AddListener(OnClickCopy);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClickCopy()
    {
        // 다음 단계에서 실제 복사 기능을 구현할 예정
        FurniturePiece selected = FurnitureManager.Instance.GetSelected();
        if (selected == null) return;

        Debug.Log("[Copy] 복사 기능은 다음 단계에서 구현됩니다.");
    }
}
