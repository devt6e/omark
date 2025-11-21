using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;

public class ApiClient : MonoBehaviour
{
    [SerializeField] public string baseUrl;
    public string AccessToken { get; set; }

    // 인증 헤더 추가
    private void AddAuth(UnityWebRequest req)
    {
        if (!string.IsNullOrEmpty(AccessToken))
            req.SetRequestHeader("Authorization", "Bearer " + AccessToken);
    }

    // ---------------------------
    // GET
    // ---------------------------
    public IEnumerator Get(string path, Action<bool, string> done)
    {
        var req = UnityWebRequest.Get(baseUrl + path);
        AddAuth(req);

        yield return req.SendWebRequest();
        bool ok = req.result == UnityWebRequest.Result.Success;
        done(ok, req.downloadHandler?.text);
    }

    // ============================================================
    // POST 오버로드 (1) — AuthService에서 사용하는 4개 인수 버전
    // ============================================================
    public IEnumerator Post(string path, string json, Action<bool, string> done, bool auth)
    {
        var req = new UnityWebRequest(baseUrl + path, UnityWebRequest.kHttpVerbPOST);
        var raw = Encoding.UTF8.GetBytes(json ?? "{}");

        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        if (auth) AddAuth(req);

        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;
        done(ok, req.downloadHandler?.text);
    }

    // ============================================================
    // POST 오버로드 (2) — 공간관리 API용 3개 인수 버전
    // ============================================================
    public IEnumerator Post(string path, string json, Action<bool, string> done)
    {
        var req = new UnityWebRequest(baseUrl + path, UnityWebRequest.kHttpVerbPOST);
        var raw = Encoding.UTF8.GetBytes(json ?? "{}");

        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        // 공간관리 API는 인증 필요
        AddAuth(req);

        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;
        done(ok, req.downloadHandler?.text);
    }

    // ---------------------------
    // PUT
    // ---------------------------
    public IEnumerator Put(string path, string json, Action<bool, string> done)
    {
        var req = new UnityWebRequest(baseUrl + path, UnityWebRequest.kHttpVerbPUT);
        var raw = Encoding.UTF8.GetBytes(json ?? "{}");

        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        AddAuth(req);

        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;
        done(ok, req.downloadHandler?.text);
    }

    // ---------------------------
    // DELETE
    // ---------------------------
    public IEnumerator Delete(string path, Action<bool, string> done)
    {
        var req = UnityWebRequest.Delete(baseUrl + path);
        AddAuth(req);

        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;
        done(ok, req.downloadHandler?.text);
    }
}
