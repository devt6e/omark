using UnityEngine;

public class SelectSpaceManager : MonoBehaviour
{
    public static SelectSpaceManager Instance;
    public GameObject targetPanel;

    void Awake()
    {
        Instance = this;
    }
}
