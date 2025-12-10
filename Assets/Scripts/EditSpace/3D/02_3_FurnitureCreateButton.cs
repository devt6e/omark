using UnityEngine;
using UnityEngine.UI;

public class FurnitureCreateButton : MonoBehaviour
{
    public Button createButton;
    public FurnitureSizePopup popup;

    private void Awake()
    {
        createButton.onClick.AddListener(OnClickCreate);
    }

    private void OnClickCreate()
    {
        popup.Show(OnSizeConfirmed);
    }

    private void OnSizeConfirmed(Vector3 size)
    {
        FurnitureSpawner3D.Instance.SpawnFurniture(size);
    }
}
