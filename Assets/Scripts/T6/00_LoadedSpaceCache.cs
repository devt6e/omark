/// <summary>
/// 씬 전환용 캐시
/// - 메인 → 편집/VR 씬으로 넘어갈 때 필요한 최소 정보만 유지
/// - 실제 공간 데이터는 SpaceSaveFileDto(SPACE.json)로만 처리
/// </summary>
public static class LoadedSpaceCache
{
    /// <summary>
    /// 서버 Environment ID
    /// </summary>
    public static long EnvironmentId { get; set; }

    /// <summary>
    /// 메인화면/리스트용 요약 정보
    /// (파일 목록 없음)
    /// </summary>
    public static T6SpaceSummary Summary { get; set; }

    /// <summary>
    /// 편집/VR 씬에서 사용되는 실제 공간 데이터
    /// (= SPACE.json 역직렬화 결과)
    /// </summary>
    public static SpaceSaveFileDto SpaceData { get; set; }

    public static bool HasEnvironment => EnvironmentId > 0;

    public static void Clear()
    {
        EnvironmentId = 0;
        Summary = null;
        SpaceData = null;
    }
}
