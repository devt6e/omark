using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AccountRecoveryApi : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost:8080";

    // -----------------------------------------------------------
    // 공통 API 응답 구조
    // -----------------------------------------------------------
    [Serializable]
    public class ApiResponse
    {
        public string status;   // "success" or "fail"
        public string message;  // 안내문
        public ResponseData data;
    }

    // data 안에 askId 또는 email이 들어오기 때문에 둘 다 포함
    [Serializable]
    public class ResponseData
    {
        public int askId;     // 1단계에서 사용
        public string email;  // 2단계에서 사용
    }

    // -----------------------------------------------------------
    // 1단계 요청 DTO
    // -----------------------------------------------------------
    [Serializable]
    public class FindAskIdRequest
    {
        public string phoneNumber;
    }

    // -----------------------------------------------------------
    // 2단계 요청 DTO
    // -----------------------------------------------------------
    [Serializable]
    public class FindEmailRequest
    {
        public string phoneNumber;
        public int askId;
        public string askAnswer;
    }

    // -----------------------------------------------------------
    // 1단계: 전화번호로 askId 요청
    // -----------------------------------------------------------
    public void FindAskId(string phoneNumber,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        FindAskIdRequest req = new FindAskIdRequest
        {
            phoneNumber = phoneNumber
        };

        StartCoroutine(PostJson("/api/v1/auth/find-ask-id", req, onCompleted, onError));
    }

    // -----------------------------------------------------------
    // 2단계: 질문답변으로 email 요청
    // -----------------------------------------------------------
    public void FindEmail(string phoneNumber, int askId, string askAnswer,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        FindEmailRequest req = new FindEmailRequest
        {
            phoneNumber = phoneNumber,
            askId = askId,
            askAnswer = askAnswer
        };

        StartCoroutine(PostJson("/api/v1/auth/find-email", req, onCompleted, onError));
    }

    // -----------------------------------------------------------
    // 공통 POST JSON 처리
    // -----------------------------------------------------------
    IEnumerator PostJson(string path, object body,
        Action<ApiResponse> onCompleted,
        Action<string> onError)
    {
        string url = baseUrl + path;
        string json = JsonUtility.ToJson(body);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        string responseText = request.downloadHandler.text;
        ApiResponse res;

        try
        {
            res = JsonUtility.FromJson<ApiResponse>(responseText);
        }
        catch (Exception e)
        {
            onError?.Invoke("Parse error: " + e.Message);
            yield break;
        }

        if (res == null)
        {
            onError?.Invoke("Empty response");
            yield break;
        }

        onCompleted?.Invoke(res);
    }
}
