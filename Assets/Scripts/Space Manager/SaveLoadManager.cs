using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void RequestSave()
    {
        UIManager.Instance.OpenSaveConfirmPopup();
    }

    public void Save()
    {
        Debug.Log("Save Data");
    }
}
