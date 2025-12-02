using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject panelLeftMenu;
    public GameObject panelDropdown;
    public GameObject panelHelpPopup;
    public GameObject panelSaveConfirmPopup;
    public GameObject blocker;

    public TextMeshProUGUI txtModeTitle;
    public TextMeshProUGUI txtRoomName;

    private void Awake()
    {
        Instance = this;
        panelDropdown.SetActive(false);
        panelHelpPopup.SetActive(false);
        panelSaveConfirmPopup.SetActive(false);
        blocker.SetActive(false);
    }

    public void ToggleDropdown()
    {
        bool active = !panelDropdown.activeSelf;
        panelDropdown.SetActive(active);
    }

    public void UpdateModeTitle(string title)
    {
        txtModeTitle.text = title;
    }

    public void UpdateViewModeIcon(string mode)
    {
        txtRoomName.text = mode;
    }

    public void OpenHelpPopup()
    {
        panelHelpPopup.SetActive(true);
        blocker.SetActive(true);
    }

    public void CloseHelpPopup()
    {
        panelHelpPopup.SetActive(false);
        blocker.SetActive(false);
    }

    public void OpenSaveConfirmPopup()
    {
        panelSaveConfirmPopup.SetActive(true);
        blocker.SetActive(true);
    }

    public void CloseSaveConfirmPopup()
    {
        panelSaveConfirmPopup.SetActive(false);
        blocker.SetActive(false);
    }
}
