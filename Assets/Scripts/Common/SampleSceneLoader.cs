using UnityEngine;

/// <summary>
/// RoomItem에서 클릭 시 sample 씬으로 이동할 때,
/// environmentId와 s3FileUrl을 넘기기 위한 Loader.
/// sample 씬은 이 값을 기반으로 S3 데이터를 가져오거나
/// 초기 공간을 구성할 수 있다.
/// </summary>
public static class SampleSceneLoader
{
    // RoomItem.cs에서 세팅됨
    public static long CurrentEnvironmentId { get; private set; }
    public static string CurrentS3FileUrl { get; private set; }

    /// <summary>
    /// sample 씬으로 이동하기 전에 RoomItem이 호출하는 함수
    /// </summary>
    public static void Load(long envId, string s3FileUrl)
    {
        CurrentEnvironmentId = envId;
        CurrentS3FileUrl = s3FileUrl;
    }

    /// <summary>
    /// sample 씬에서 현재 환경 ID를 얻어오기
    /// </summary>
    public static long GetEnvironmentId()
    {
        return CurrentEnvironmentId;
    }

    /// <summary>
    /// sample 씬에서 S3 파일 URL을 얻어오기
    /// </summary>
    public static string GetS3FileUrl()
    {
        return CurrentS3FileUrl;
    }

    /// <summary>
    /// sample 씬 로딩 시작 시 초기화해야 할 사항이 있다면 확장 가능
    /// </summary>
    public static void Clear()
    {
        CurrentEnvironmentId = 0;
        CurrentS3FileUrl = null;
    }
}
