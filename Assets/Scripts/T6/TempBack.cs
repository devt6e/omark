using UnityEngine;
using UnityEngine.SceneManagement;

public class TempBack : MonoBehaviour
{
    [SerializeField] private string nextScene;  // 이동할 씬 이름

    public void Go()
    {
        SceneManager.LoadScene(nextScene);
    }
}
