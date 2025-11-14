using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;

public class ApiClient : MonoBehaviour
{
    [SerializeField] string baseUrl;
    public string AccessToken { get; set; }

    public IEnumerator Get(string path, Action<bool, string> done)
    {
        var req = UnityWebRequest.Get(baseUrl + path);
        if (!string.IsNullOrEmpty(AccessToken)) req.SetRequestHeader("Authorization", "Bearer " + AccessToken);
        yield return req.SendWebRequest();
        done(req.result == UnityWebRequest.Result.Success, req.downloadHandler.text);
    }

    public IEnumerator Post(string path, string json, Action<bool, string> done, bool auth = false)
    {
        var req = new UnityWebRequest(baseUrl + path, "POST");
        var raw = Encoding.UTF8.GetBytes(json ?? "{}");
        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        if (auth && !string.IsNullOrEmpty(AccessToken)) req.SetRequestHeader("Authorization", "Bearer " + AccessToken);
        yield return req.SendWebRequest();
        done(req.result == UnityWebRequest.Result.Success, req.downloadHandler.text);
    }
}
