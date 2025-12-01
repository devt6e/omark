using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceSceneManager : MonoBehaviour
{
    public static SpaceSceneManager Instance;
    public string currentRoomName;
    public int currentRoomId;

    void Awake()
    {
        Instance = this;
    }

    public void LoadSpace(int roomId, string roomName)
    {
        currentRoomId = roomId;
        currentRoomName = roomName;
        SceneManager.LoadScene("space");
    }

    public void ExitToMain()
    {
        SceneManager.LoadScene("sc_main");
    }
}
