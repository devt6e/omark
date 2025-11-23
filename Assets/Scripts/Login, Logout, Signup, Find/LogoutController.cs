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
        if (!auth) auth = FindAnyObjectByType<AuthService>();
        StopAllCoroutines();
        StartCoroutine(CoLogout());
    }

    IEnumerator CoLogout()
    {
        if (txtFeedback) txtFeedback.text = "�α׾ƿ� ���Դϴ�...";
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
                ? "���� �α׾ƿ��� �����߽��ϴ�. �ٽ� �õ��� �ּ���."
                : msg;
        }

        SceneManager.LoadScene(loginSceneName);
    }
}
