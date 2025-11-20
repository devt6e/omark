using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SignupController : MonoBehaviour
{
    [SerializeField] AuthService auth;

    [SerializeField] TMP_InputField inputId;
    [SerializeField] TextMeshProUGUI txtIdError;

    [SerializeField] TMP_InputField inputPassword;
    [SerializeField] TMP_InputField inputPwCheck;
    [SerializeField] TextMeshProUGUI txtPwError;
    [SerializeField] TextMeshProUGUI txtPwCheckError;

    [SerializeField] TMP_InputField inputName;
    [SerializeField] TMP_InputField inputPhoneNumber;
    [SerializeField] TMP_InputField inputEmail;
    [SerializeField] TMP_InputField inputAskAnswer;

    [SerializeField] TMP_Dropdown dropdownQuestions;

    [SerializeField] string loginSceneName = "sc_login";

    List<SecurityQuestionResponseDto> questionList;
    bool idChecked;
    string checkedEmail;

    void Start()
    {
        if (!auth) auth = FindObjectOfType<AuthService>();
        StartCoroutine(LoadQuestions());
    }

    IEnumerator LoadQuestions()
    {
        if (txtIdError) txtIdError.text = "보안 질문을 불러오는 중입니다...";

        if (auth == null)
        {
            if (txtIdError) txtIdError.text = "AuthService가 설정되지 않았습니다.";
            yield break;
        }

        bool finished = false;
        string error = null;
        List<SecurityQuestionResponseDto> list = null;

        yield return auth.GetSecurityQuestions(
            l =>
            {
                list = l;
                finished = true;
            },
            err =>
            {
                error = err;
                finished = true;
            });

        if (!finished)
            yield break;

        if (!string.IsNullOrEmpty(error))
        {
            if (txtIdError) txtIdError.text = error;
            yield break;
        }

        questionList = list;

        if (dropdownQuestions)
        {
            dropdownQuestions.options.Clear();
            foreach (var q in questionList)
            {
                dropdownQuestions.options.Add(new TMP_Dropdown.OptionData(q.questionText));
            }
            dropdownQuestions.RefreshShownValue();
        }

        if (txtIdError) txtIdError.text = "";
    }

    public void CheckId()
    {
        if (!auth) auth = FindObjectOfType<AuthService>();
        StopCoroutine(nameof(CoCheckId));
        StartCoroutine(CoCheckId());
    }

    public void OnClickCheckId()
    {
        CheckId();
    }

    IEnumerator CoCheckId()
    {
        if (txtIdError) txtIdError.text = "";

        string email = inputId ? inputId.text.Trim() : "";

        if (string.IsNullOrEmpty(email))
        {
            if (txtIdError) txtIdError.text = "아이디(이메일)를 입력하세요.";
            idChecked = false;
            yield break;
        }

        if (!IsValidEmail(email))
        {
            if (txtIdError) txtIdError.text = "이메일 형식이 올바르지 않습니다.";
            idChecked = false;
            yield break;
        }

        bool ok = false;
        string msg = null;

        yield return auth.CheckEmail(email, (success, m) =>
        {
            ok = success;
            msg = m;
        });

        if (ok)
        {
            idChecked = true;
            checkedEmail = email;
            if (txtIdError) txtIdError.text = string.IsNullOrEmpty(msg) ? "사용 가능한 아이디입니다." : msg;
        }
        else
        {
            idChecked = false;
            if (txtIdError) txtIdError.text = string.IsNullOrEmpty(msg) ? "이미 사용 중인 아이디입니다." : msg;
        }
    }

    public void OnPasswordChanged(string _)
    {
        string pw = inputPassword ? inputPassword.text : "";
        ValidatePassword(pw);
    }

    public void OnPasswordCheckChanged(string _)
    {
        if (!inputPassword || !inputPwCheck)
            return;

        string pw = inputPassword.text;
        string pw2 = inputPwCheck.text;

        if (pw2.Length == 0)
        {
            if (txtPwCheckError) txtPwCheckError.text = "";
            return;
        }

        if (pw != pw2)
        {
            if (txtPwCheckError) txtPwCheckError.text = "비밀번호가 일치하지 않습니다.";
        }
        else
        {
            if (txtPwCheckError) txtPwCheckError.text = "";
        }
    }

    public void OnClickSignUp()
    {
        StopCoroutine(nameof(CoSignUp));
        StartCoroutine(CoSignUp());
    }

    IEnumerator CoSignUp()
    {
        string email = inputId ? inputId.text.Trim() : "";
        string pw = inputPassword ? inputPassword.text : "";
        string pw2 = inputPwCheck ? inputPwCheck.text : "";
        string name = inputName ? inputName.text.Trim() : "";
        string phone = inputPhoneNumber ? inputPhoneNumber.text.Trim() : "";
        string answer = inputAskAnswer ? inputAskAnswer.text.Trim() : "";

        if (!IsValidEmail(email))
        {
            if (txtIdError) txtIdError.text = "이메일 형식이 올바르지 않습니다.";
            yield break;
        }

        if (!idChecked || checkedEmail != email)
        {
            if (txtIdError) txtIdError.text = "아이디 중복확인을 먼저 해주세요.";
            yield break;
        }

        if (!ValidatePassword(pw))
            yield break;

        if (pw != pw2)
        {
            if (txtPwCheckError) txtPwCheckError.text = "비밀번호가 일치하지 않습니다.";
            yield break;
        }

        if (string.IsNullOrEmpty(name) ||
            string.IsNullOrEmpty(phone) ||
            string.IsNullOrEmpty(answer))
        {
            if (txtIdError) txtIdError.text = "이름, 휴대전화번호, 질문 답변을 모두 입력하세요.";
            yield break;
        }

        if (questionList == null || questionList.Count == 0)
        {
            if (txtIdError) txtIdError.text = "보안 질문을 불러오지 못했습니다.";
            yield break;
        }

        int idx = dropdownQuestions ? dropdownQuestions.value : 0;
        long askId = questionList[idx].id;

        var dto = new SignUpRequestDto
        {
            email = email,
            password = pw,
            name = name,
            phoneNumber = phone,
            askId = askId,
            askAnswer = answer
        };

        if (txtIdError) txtIdError.text = "회원가입 요청 중입니다...";

        bool ok = false;
        string err = null;

        yield return auth.SignUp(
            dto,
            () =>
            {
                ok = true;
            },
            e =>
            {
                ok = false;
                err = e;
            });

        if (!ok)
        {
            if (txtIdError) txtIdError.text = string.IsNullOrEmpty(err) ? "회원가입에 실패했습니다." : err;
            yield break;
        }

        if (txtIdError) txtIdError.text = "회원가입 성공! 로그인 화면으로 이동합니다.";
        yield return new WaitForSeconds(1f);

        if (!string.IsNullOrEmpty(loginSceneName))
        {
            SceneManager.LoadScene(loginSceneName);
        }
    }

    bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return email.Contains("@") && email.Contains(".");
    }

    bool ValidatePassword(string pw)
    {
        if (txtPwError) txtPwError.text = "";

        if (string.IsNullOrEmpty(pw))
        {
            if (txtPwError) txtPwError.text = "비밀번호를 입력하세요.";
            return false;
        }

        if (pw.Length < 10 || pw.Length > 20)
        {
            if (txtPwError) txtPwError.text = "비밀번호는 10~20자여야 합니다.";
            return false;
        }

        if (pw.Contains(" "))
        {
            if (txtPwError) txtPwError.text = "공백 문자는 사용할 수 없습니다.";
            return false;
        }

        bool hasDigit = Regex.IsMatch(pw, "[0-9]");
        bool hasLetter = Regex.IsMatch(pw, "[A-Za-z]");
        bool hasSpecial = Regex.IsMatch(pw, "[^A-Za-z0-9]");

        if (!hasDigit || !hasLetter || !hasSpecial)
        {
            if (txtPwError) txtPwError.text = "영문, 숫자, 특수문자를 모두 포함해야 합니다.";
            return false;
        }

        return true;
    }
}
