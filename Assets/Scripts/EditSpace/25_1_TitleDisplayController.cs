using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TitleDisplayController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TitleEditPopup popup;  

    public void OnPointerClick(PointerEventData eventData)
    {
        popup.Open(
            currentTitle: titleText.text,
            confirmCallback: (newTitle) =>
            {
                titleText.text = newTitle;
                // 필요하면 저장 로직도 추가 가능
            },
            cancelCallback: () =>
            {
                // 취소 시 특별한 처리 필요 없으면 비워둠
            });
    }
}
