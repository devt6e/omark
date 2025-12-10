using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    public GameObject panelHelpPopup;
    public Button btnQuestion;
    public Button btnClose;

    public Image[] helpContents;
    public Button[] dots;
    public Image[] dotImages;

    public Color activeColor = new Color32(0x2D, 0x21, 0x09, 0xFF);
    public Color inactiveColor = new Color32(0xFF, 0xF6, 0xEF, 0xFF);

    int currentIndex = 0;

    void Start()
    {
        btnQuestion.onClick.AddListener(OpenHelpPopup);
        btnClose.onClick.AddListener(CloseHelpPopup);

        for (int i = 0; i < dots.Length; i++)
        {
            int index = i;
            dots[i].onClick.AddListener(() => OnDotClick(index));
        }

        InitPopup();
    }

    void InitPopup()
    {
        panelHelpPopup.SetActive(false);
        ShowContent(0);
        UpdateDots(0);
    }

    void OpenHelpPopup()
    {
        panelHelpPopup.SetActive(true);
        ShowContent(0);
        UpdateDots(0);
    }

    void CloseHelpPopup()
    {
        panelHelpPopup.SetActive(false);
    }

    void OnDotClick(int index)
    {
        ShowContent(index);
        UpdateDots(index);
    }

    void ShowContent(int index)
    {
        currentIndex = index;
        for (int i = 0; i < helpContents.Length; i++)
            helpContents[i].gameObject.SetActive(i == index);
    }

    void UpdateDots(int index)
    {
        for (int i = 0; i < dotImages.Length; i++)
            dotImages[i].color = (i == index) ? activeColor : inactiveColor;
    }
}
