using UnityEngine;
using UnityEngine.UI;

public class MarkerCreateButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject createPanel;
    [SerializeField] private GameObject Inventory;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            createPanel.SetActive(true);
            Inventory.SetActive(false);
        });
    }
}
