using System;

[Serializable]
public class T6SpaceSummary
{
    public long id;            // 서버 Environment ID
    public string name;        // 공간 이름
    public string s3FileUrl;   // 공간 데이터(JSON) 위치
    public string createdAt;   // 서버가 주지 않아도 클라이언트가 직접 기록 가능

    public T6SpaceSummary() { }

    public T6SpaceSummary(long id, string name, string url, string createdAt)
    {
        this.id = id;
        this.name = name;
        this.s3FileUrl = url;
        this.createdAt = createdAt;
    }
}
