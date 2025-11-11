using UnityEngine;

public class SceneControllerLogin : MonoBehaviour
{
    public GameObject panel_quit;

    void Awake()
    {
        if (panel_quit != null)
            panel_quit.SetActive(false);
        else
            Debug.LogWarning("PanelInitializer: panelToHide is not assigned.");
    }
}
