using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadSpaceScene()
    {
        SceneManager.LoadScene("sc_in_space");
    }
    public void LoadBuildScene()
    {
        SceneManager.LoadScene("sc_in_build");
    }
    public void LoadMainScene()
    {
        SceneManager.LoadScene("sc_main");
    }
}
