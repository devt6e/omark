using UnityEngine;
using System;
using System.Collections;

public class AuthService : MonoBehaviour
{
    [SerializeField] ApiClient api;

    public IEnumerator Login(string email, string password, Action<bool, string> done)
    {
        var req = new LoginReq { email = email, password = password };
        var json = JsonUtility.ToJson(req);
        bool ok = false; string body = null;
        yield return api.Post("/api/v1/auth/login", json, (s, b) => { ok = s; body = b; });
        if (ok)
        {
            var res = JsonUtility.FromJson<LoginRes>(body);
            if (!string.IsNullOrEmpty(res.access_token) && !string.IsNullOrEmpty(res.refresh_token))
            {
                api.AccessToken = res.access_token;
                PlayerPrefs.SetString("USER_EMAIL", email);
                PlayerPrefs.SetString("ACCESS_TOKEN", res.access_token);
                PlayerPrefs.SetString("REFRESH_TOKEN", res.refresh_token);
                PlayerPrefs.Save();
                done(true, null);
                yield break;
            }
        }
        done(false, body);
    }
}
