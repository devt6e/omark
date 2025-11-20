using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SampleSceneDataLoader : MonoBehaviour
{
    private void Start()
    {
        long envId = SampleSceneLoader.GetEnvironmentId();
        string s3Url = SampleSceneLoader.GetS3FileUrl();

        Debug.Log($"[SampleScene] 환경 ID = {envId}");
        Debug.Log($"[SampleScene] S3 URL = {s3Url}");

        if (!string.IsNullOrEmpty(s3Url))
        {
            // 서버에 업로드된 공간 파일 다운로드
            StartCoroutine(DownloadEnvironmentData(s3Url));
        }
        else
        {
            Debug.Log("[SampleScene] 새로운 공간 - s3FileUrl 없음. 기본 템플릿 유지.");
        }
    }

    IEnumerator DownloadEnvironmentData(string url)
    {
        Debug.Log("[SampleScene] S3 파일 다운로드 시도: " + url);

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[SampleScene] S3 다운로드 실패: " + req.error);
            yield break;
        }

        byte[] data = req.downloadHandler.data;
        Debug.Log($"[SampleScene] 다운로드 성공! 데이터 크기: {data.Length} bytes");

        // TODO: 이 byte[]로 "가상 공간 구성" 로직을 구현하면 됨
        // 현재는 테스트 단계이므로 로그만 출력
        ApplyEnvironmentData(data);
    }

    private void ApplyEnvironmentData(byte[] data)
    {
        // 실제 구현은 data parsing → 오브젝트 생성/위치 로드 등
        // 예시로 현재는 단순 로그 출력
        Debug.Log("[SampleScene] 환경 데이터 적용 준비됨.");

        // TODO: 공간 파싱 및 오브젝트 생성 로직 구현 예정
        // 예)
        // SpaceData parsed = JsonUtility.FromJson<SpaceData>(Encoding.UTF8.GetString(data));
        // BuildObjects(parsed);
    }
}
