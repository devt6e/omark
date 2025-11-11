using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    public GameObject panel;

    public void TogglePanel_()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
        else
            Debug.LogWarning("TogglePanelOnClick: panel is not assigned.");
    }
}