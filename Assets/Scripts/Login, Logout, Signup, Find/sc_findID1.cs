using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class sc_findID1 : MonoBehaviour
{
    public TMP_InputField inputPhone;
    public TextMeshProUGUI txtError;
    public AccountRecoveryApi api;

    public void OnClickNext()
    {
        string phone = inputPhone.text.Trim();

        if (string.IsNullOrEmpty(phone))
        {
            txtError.text = "전화번호를 입력해주세요.";
            return;
        }

        api.FindAskId(phone,
            (res) =>
            {
                // 실패 응답 처리
                if (res.status != "success")
                {
                    txtError.text = res.message;
                    return;
                }

                if (res.data == null || res.data.askId <= 0)
                {
                    txtError.text = "가입되지 않은 번호입니다.";
                    return;
                }

                // 🔥 디버그 로그 — askId가 API로부터 제대로 왔는지 확인
                Debug.Log("📌 [sc_findID1] 저장 직전 askId = " + res.data.askId);

                // 저장
                AccountRecoverySession.PhoneNumber = phone;
                AccountRecoverySession.AskId = res.data.askId;

                // 다음 씬 이동
                SceneManager.LoadScene("sc_findID2");
            },
            (err) =>
            {
                txtError.text = err;
            });
    }
}
