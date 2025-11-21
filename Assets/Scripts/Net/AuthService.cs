using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AuthService : MonoBehaviour
{
    [SerializeField] ApiClient api;

    public IEnumerator GetSecurityQuestions(Action<List<SecurityQuestionResponseDto>> onSuccess, Action<string> onError)
    {
        bool ok = false;
        string body = null;

        yield return api.Get("/api/v1/auth/questions", (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("보안 질문 목록을 불러오지 못했습니다.");
            yield break;
        }

        QuestionsResponse res = null;

        try
        {
            res = JsonUtility.FromJson<QuestionsResponse>(body);
        }
        catch
        {
        }

        if (res != null && res.data != null && res.data.Count > 0)
        {
            onSuccess?.Invoke(res.data);
        }
        else
        {
            string msg = res != null && !string.IsNullOrEmpty(res.message)
                ? res.message
                : "보안 질문 목록이 비어 있습니다.";
            onError?.Invoke(msg);
        }
    }

    public IEnumerator SignUp(SignUpRequestDto dto, Action onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(dto);

        bool ok = false;
        string body = null;

        yield return api.Post("/api/v1/auth/signup", json, (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            onError?.Invoke("회원가입 요청에 실패했습니다.");
            yield break;
        }

        VoidResponse res = null;

        try
        {
            res = JsonUtility.FromJson<VoidResponse>(body);
        }
        catch
        {
        }

        if (res != null && res.status == "success")
        {
            onSuccess?.Invoke();
        }
        else
        {
            string msg = res != null && !string.IsNullOrEmpty(res.message)
                ? res.message
                : "회원가입에 실패했습니다.";
            onError?.Invoke(msg);
        }
    }

    public IEnumerator Login(string email, string password, Action<bool, string> done)
    {
        var req = new LoginRequestDto
        {
            email = email,
            password = password
        };

        string json = JsonUtility.ToJson(req);

        bool ok = false;
        string body = null;

        yield return api.Post("/api/v1/auth/login", json, (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            done(false, "로그인 요청에 실패했습니다.");
            yield break;
        }

        LoginResponseWrapper res = null;

        try
        {
            res = JsonUtility.FromJson<LoginResponseWrapper>(body);
        }
        catch
        {
        }

        if (res != null && res.status == "success" && res.data != null &&
            !string.IsNullOrEmpty(res.data.access_token) &&
            !string.IsNullOrEmpty(res.data.refresh_token))
        {
            api.AccessToken = res.data.access_token;

            PlayerPrefs.SetString("USER_EMAIL", email);
            PlayerPrefs.SetString("ACCESS_TOKEN", res.data.access_token);
            PlayerPrefs.SetString("REFRESH_TOKEN", res.data.refresh_token);
            PlayerPrefs.Save();

            done(true, null);
        }
        else
        {
            string msg = res != null && !string.IsNullOrEmpty(res.message)
                ? res.message
                : "로그인에 실패했습니다.";
            done(false, msg);
        }
    }

    public IEnumerator CheckEmail(string email, Action<bool, string> done)
    {
        var req = new CheckEmailRequestDto
        {
            email = email
        };

        string json = JsonUtility.ToJson(req);

        bool ok = false;
        string body = null;

        yield return api.Post("/api/v1/auth/check-email", json, (s, b) =>
        {
            ok = s;
            body = b;
        });

        VoidResponse res = null;

        try
        {
            res = JsonUtility.FromJson<VoidResponse>(body);
        }
        catch
        {
        }

        if (ok)
        {
            string msg = res != null && !string.IsNullOrEmpty(res.message)
                ? res.message
                : null;
            done(true, msg);
        }
        else
        {
            string msg = res != null && !string.IsNullOrEmpty(res.message)
                ? res.message
                : "이미 사용 중인 아이디입니다.";
            done(false, msg);
        }
    }

    public IEnumerator RefreshToken(string email, string refreshToken, Action<bool, string> done)
    {
        var req = new RefreshTokenRequestDto
        {
            email = email,
            refreshToken = refreshToken
        };

        string json = JsonUtility.ToJson(req);

        bool ok = false;
        string body = null;

        yield return api.Post("/api/v1/auth/refresh", json, (s, b) =>
        {
            ok = s;
            body = b;
        });

        if (!ok || string.IsNullOrEmpty(body))
        {
            done(false, "토큰 갱신 요청에 실패했습니다.");
            yield break;
        }

        RefreshResponseWrapper res = null;

        try
        {
            res = JsonUtility.FromJson<RefreshResponseWrapper>(body);
        }
        catch
        {
        }

        if (res != null && res.status == "success" && res.data != null &&
            !string.IsNullOrEmpty(res.data.access_token))
        {
            api.AccessToken = res.data.access_token;

            PlayerPrefs.SetString("ACCESS_TOKEN", res.data.access_token);
            PlayerPrefs.Save();

            done(true, null);
        }
        else
        {
            string msg = res != null && !string.IsNullOrEmpty(res.message)
                ? res.message
                : "토큰 갱신에 실패했습니다.";
            done(false, msg);
        }
    }

    public IEnumerator Logout(Action<bool, string> done)
    {
        bool ok = false;
        string body = null;

        yield return api.Post("/api/v1/auth/logout", "{}", (s, b) =>
        {
            ok = s;
            body = b;
        }, true);

        PlayerPrefs.DeleteKey("USER_EMAIL");
        PlayerPrefs.DeleteKey("ACCESS_TOKEN");
        PlayerPrefs.DeleteKey("REFRESH_TOKEN");
        PlayerPrefs.SetInt("AUTO_LOGIN", 0);
        PlayerPrefs.Save();
        api.AccessToken = null;

        if (!ok || string.IsNullOrEmpty(body))
        {
            done(false, "서버 로그아웃 요청에 실패했습니다.");
            yield break;
        }

        VoidResponse res = null;

        try
        {
            res = JsonUtility.FromJson<VoidResponse>(body);
        }
        catch
        {
        }

        if (res != null && res.status == "success")
        {
            done(true, null);
        }
        else
        {
            string msg = res != null && !string.IsNullOrEmpty(res.message)
                ? res.message
                : "로그아웃 처리 중 오류가 발생했습니다.";
            done(false, msg);
        }
    }
}
