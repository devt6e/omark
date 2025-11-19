using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoutController : MonoBehaviour
{
    [SerializeField] AuthService auth;
    [SerializeField] TextMeshProUGUI txtFeedback;
    [SerializeField] GameObject loading;
    [SerializeField] string loginSceneName = "sc_login";

    public void OnClickLogout()
    {
        if (!auth) auth = FindObjectOfType<AuthService>();
        StopAllCoroutines();
        StartCoroutine(CoLogout());
    }

    IEnumerator CoLogout()
    {
        if (txtFeedback) txtFeedback.text = "로그아웃 중입니다...";
        if (loading) loading.SetActive(true);

        bool ok = false;
        string msg = null;

        yield return auth.Logout((success, message) =>
        {
            ok = success;
            msg = message;
        });

        if (loading) loading.SetActive(false);

        if (!ok && txtFeedback)
        {
            txtFeedback.text = string.IsNullOrEmpty(msg)
                ? "서버 로그아웃에 실패했습니다. 다시 시도해 주세요."
                : msg;
        }

        SceneManager.LoadScene(loginSceneName);
    }
}
