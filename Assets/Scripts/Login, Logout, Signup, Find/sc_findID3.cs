using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class sc_findID3 : MonoBehaviour
{
    public TextMeshProUGUI txtEmail;

    void Start()
    {
        txtEmail.text = AccountRecoverySession.Email;
    }

    public void OnClickGoLogin()
    {
        SceneManager.LoadScene("sc_login");
    }
}
