using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNav : MonoBehaviour
{
    [SerializeField] string sceneName;
    public void Load() { if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName); }
}
