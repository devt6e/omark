using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryScroll : MonoBehaviour
{
    public static InventoryScroll Instance { get; private set; }
    public bool allowScroll = true;

    [SerializeField] private ScrollRect scrollRect;

    private void Awake()
    {
        Instance = this;
    }

    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     if (!allowScroll)
    //         eventData.Use();
    // }


    // 스크롤 ON/OFF
    public void SetScroll(bool canScroll)
    {
        scrollRect.vertical = canScroll;
        scrollRect.horizontal = false; // 이 프로젝트 기준
    }
}
