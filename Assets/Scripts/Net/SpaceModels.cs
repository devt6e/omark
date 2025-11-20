using System;
using System.Collections.Generic;

// 서버의 표준 응답 구조
[Serializable]
public class ApiResponse<T>
{
    public string status;
    public string message;
    public T data;
}

// 1) 공간 생성/수정 요청 DTO
[Serializable]
public class VirtualEnvironmentRequestDto
{
    public string name;
}

// 2) 공간 응답 DTO
[Serializable]
public class VirtualEnvironmentResponseDto
{
    public long id;
    public string name;
    public string s3FileUrl;
    public long userId;
}

// 3) Presigned URL 요청 DTO
[Serializable]
public class S3PresignedUrlRequestDto
{
    public string fileName;
}

// 4) Presigned URL 응답 DTO
[Serializable]
public class S3PresignedUrlResponseDto
{
    public string presignedUploadUrl;
    public string finalFileUrl;
}
