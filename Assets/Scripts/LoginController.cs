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
        if (toggleAuto) toggleAuto.isOn = PlayerPrefs.GetInt("AUTO_LOGIN", 0) == 1;
    }

    public void OnClickLogin()
    {
        StopAllCoroutines();
        StartCoroutine(CoLogin());
    }

    IEnumerator CoLogin()
    {
        if (errorText) errorText.text = "";
        var id = inputId ? inputId.text.Trim() : "";
        var pw = inputPw ? inputPw.text : "";
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            if (errorText) errorText.text = "아이디와 비밀번호를 입력하세요.";
            yield break;
        }

        if (loading) loading.SetActive(true);
        bool ok = false; string msg = null;
        yield return auth.Login(id, pw, (success, m) => { ok = success; msg = m; });
        if (loading) loading.SetActive(false);

        if (!ok)
        {
            if (errorText) errorText.text = msg != null && msg.ToLower().Contains("id") ? "존재하지 않는 아이디입니다." : "잘못된 비밀번호입니다.";
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

    public void ClearError()
    {
        if (errorText) errorText.text = "";
    }
    public void ClearError(string _) { ClearError(); }

}
