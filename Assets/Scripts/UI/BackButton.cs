using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] string targetScene = "sc_login";
    [SerializeField] Button button;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (button) button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (!string.IsNullOrEmpty(targetScene)) SceneManager.LoadScene(targetScene);
        else
        {
            var i = SceneManager.GetActiveScene().buildIndex;
            if (i > 0) SceneManager.LoadScene(i - 1);
        }
    }
}
