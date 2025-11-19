using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceApi : MonoBehaviour
{
    [SerializeField] ApiClient api;

    // ------------------------
    // GET 모든 공간
    // ------------------------
    public IEnumerator GetAllEnvironments(
        System.Action<List<VirtualEnvironmentResponseDto>> onSuccess,
        System.Action<string> onError)
    {
        yield return api.Get("/api/v1/environments", (ok, body) =>
        {
            if (!ok)
            {
                onError?.Invoke("공간 목록 요청 실패");
                return;
            }

            // body가 비어있어도 기본 구조를 만들어 처리
            if (string.IsNullOrEmpty(body) || body == "{}")
            {
                onSuccess?.Invoke(new List<VirtualEnvironmentResponseDto>());
                return;
            }

            ApiResponse<List<VirtualEnvironmentResponseDto>> res =
                JsonUtility.FromJson<ApiResponse<List<VirtualEnvironmentResponseDto>>>(body);

            // res 또는 res.data 가 null 일 수 있음
            if (res != null && res.status == "success")
            {
                if (res.data == null)
                    onSuccess?.Invoke(new List<VirtualEnvironmentResponseDto>());
                else
                    onSuccess?.Invoke(res.data);
            }
            else
            {
                onError?.Invoke(res?.message ?? "목록 응답 오류");
            }
        });
    }


    // ------------------------
    // POST 새로운 공간 생성
    // ------------------------
    public IEnumerator CreateEnvironment(
        string name,
        System.Action<VirtualEnvironmentResponseDto> onSuccess,
        System.Action<string> onError)
    {
        var dto = new VirtualEnvironmentRequestDto { name = name };
        string json = JsonUtility.ToJson(dto);

        yield return api.Post("/api/v1/environments", json, (ok, body) =>
        {
            if (!ok)
            {
                onError?.Invoke("공간 생성 실패");
                return;
            }

            ApiResponse<VirtualEnvironmentResponseDto> res =
                JsonUtility.FromJson<ApiResponse<VirtualEnvironmentResponseDto>>(body);

            if (res != null && res.status == "success")
                onSuccess?.Invoke(res.data);
            else
                onError?.Invoke(res?.message ?? "생성 오류");
        });
    }


    // ------------------------
    // PUT 이름 수정
    // ------------------------
    public IEnumerator UpdateEnvironment(
        long envId,
        string newName,
        System.Action onSuccess,
        System.Action<string> onError)
    {
        var dto = new VirtualEnvironmentRequestDto { name = newName };
        string json = JsonUtility.ToJson(dto);

        yield return api.Put($"/api/v1/environments/{envId}", json, (ok, body) =>
        {
            if (!ok)
            {
                onError?.Invoke("이름 수정 실패");
                return;
            }

            ApiResponse<VirtualEnvironmentResponseDto> res =
                JsonUtility.FromJson<ApiResponse<VirtualEnvironmentResponseDto>>(body);

            if (res != null && res.status == "success")
                onSuccess?.Invoke();
            else
                onError?.Invoke(res?.message ?? "수정 오류");
        });
    }


    // ------------------------
    // DELETE 공간 삭제
    // ------------------------
    public IEnumerator DeleteEnvironment(
        long envId,
        System.Action onSuccess,
        System.Action<string> onError)
    {
        yield return api.Delete($"/api/v1/environments/{envId}", (ok, body) =>
        {
            if (!ok)
            {
                onError?.Invoke("삭제 실패");
                return;
            }

            ApiResponse<object> res =
                JsonUtility.FromJson<ApiResponse<object>>(body);

            if (res != null && res.status == "success")
                onSuccess?.Invoke();
            else
                onError?.Invoke(res?.message ?? "삭제 오류");
        });
    }
}
