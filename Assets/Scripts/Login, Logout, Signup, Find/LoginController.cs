using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginController : MonoBehaviour
{
    [SerializeField] AuthService auth;
    [SerializeField] TMP_InputField inputId;
    [SerializeField] TMP_InputField inputPw;
    [SerializeField] Toggle toggleAuto;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] GameObject loading;
    [SerializeField] string nextSceneOnSuccess = "sc_main";

    void Awake()
    {
        if (!auth) auth = FindAnyObjectByType<AuthService>();
        if (toggleAuto) toggleAuto.isOn = PlayerPrefs.GetInt("AUTO_LOGIN", 0) == 1;
    }

    void Start()
    {
        TryAutoLogin();
    }

    void TryAutoLogin()
    {
        if (PlayerPrefs.GetInt("AUTO_LOGIN", 0) != 1)
            return;

        string email = PlayerPrefs.GetString("USER_EMAIL", "");
        string refresh = PlayerPrefs.GetString("REFRESH_TOKEN", "");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(refresh))
            return;

        StopAllCoroutines();
        StartCoroutine(CoAutoLogin(email, refresh));
    }

    public void OnClickLogin()
    {
        StopAllCoroutines();
        StartCoroutine(CoLogin());
    }

    IEnumerator CoLogin()
    {
        if (errorText) errorText.text = "";

        string id = inputId ? inputId.text.Trim() : "";
        string pw = inputPw ? inputPw.text : "";

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            if (errorText) errorText.text = "���̵�� ��й�ȣ�� �Է��ϼ���.";
            yield break;
        }

        if (loading) loading.SetActive(true);

        bool ok = false;
        string msg = null;

        yield return auth.Login(id, pw, (success, message) =>
        {
            ok = success;
            msg = message;
        });

        if (loading) loading.SetActive(false);

        if (!ok)
        {
            if (errorText) errorText.text = msg;
            yield break;
        }

        if (toggleAuto && toggleAuto.isOn)
        {
            PlayerPrefs.SetInt("AUTO_LOGIN", 1);
        }
        else
        {
            PlayerPrefs.SetInt("AUTO_LOGIN", 0);
            PlayerPrefs.DeleteKey("REFRESH_TOKEN");
            PlayerPrefs.Save();
        }

        PlayerPrefs.Save();

        SceneManager.LoadScene(nextSceneOnSuccess);
    }

    IEnumerator CoAutoLogin(string email, string refreshToken)
    {
        if (errorText) errorText.text = "�ڵ� �α��� ���Դϴ�...";
        if (loading) loading.SetActive(true);

        bool ok = false;
        string msg = null;

        yield return auth.RefreshToken(email, refreshToken, (success, message) =>
        {
            ok = success;
            msg = message;
        });

        if (loading) loading.SetActive(false);

        if (!ok)
        {
            if (errorText) errorText.text = string.IsNullOrEmpty(msg)
                ? "�ڵ� �α��ο� �����߽��ϴ�. �ٽ� �α����� �ּ���."
                : msg;

            PlayerPrefs.SetInt("AUTO_LOGIN", 0);
            PlayerPrefs.DeleteKey("ACCESS_TOKEN");
            PlayerPrefs.DeleteKey("REFRESH_TOKEN");
            PlayerPrefs.Save();
            yield break;
        }

        if (toggleAuto) toggleAuto.isOn = true;

        SceneManager.LoadScene(nextSceneOnSuccess);
    }

    public void ClearError()
    {
        if (errorText) errorText.text = "";
    }

    public void ClearError(string _)
    {
        ClearError();
    }
}
