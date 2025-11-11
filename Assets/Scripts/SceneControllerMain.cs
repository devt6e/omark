using System.Collections.Generic;
using UnityEngine;

public class SceneControllerMain : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>();

    void Awake()
    {
        foreach(GameObject panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
            else
                Debug.LogWarning("PanelInitializer: panelToHide is not assigned. name : " + panel);
        }
    }
}
