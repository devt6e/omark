// Scripts/Signup/SignupController.cs (예시)
using UnityEngine;
using TMPro;

public class SignupController : MonoBehaviour
{
    [SerializeField] TMP_InputField inputId;
    [SerializeField] TextMeshProUGUI txtIdError;

    public void CheckId()
    {
        // TODO: API 연동 전 임시 동작
        txtIdError.text = "사용 가능한 아이디입니다."; // 또는 ""
    }
}
