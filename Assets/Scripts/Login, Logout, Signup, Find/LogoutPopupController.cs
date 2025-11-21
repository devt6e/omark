using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutPopupController : MonoBehaviour
{
    [SerializeField] private GameObject popupLogout;
    [SerializeField] private string loginSceneName = "sc_login";

    public void ShowPopup()
    {
        popupLogout.SetActive(true);
    }

    public void HidePopup()
    {
        popupLogout.SetActive(false);
    }

    public void ConfirmLogout()
    {
        // TODO: 여기서 필요하면 로그아웃 처리
        SceneManager.LoadScene(loginSceneName);
    }
}
