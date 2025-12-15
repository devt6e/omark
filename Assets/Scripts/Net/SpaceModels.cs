using System;
using System.Collections.Generic;

// 1. [공통] 서버 응답 래퍼
[Serializable]
public class ApiResponse<T>
{
    public string status;
    public string message;
    public T data;
}

// 2. [요청] 공간 생성 및 이름 수정 (겸용)
[Serializable]
public class VirtualEnvironmentRequestDto
{
    public string name;
}

// 3. [응답] 공간 정보 (신버전, ★ 중요: 파일 리스트 포함됨)
[Serializable]
public class VirtualEnvironmentResponseDto
{
    public long id;
    public string name;
    public long userId;
    public List<EnvironmentFileDto> files; // 🚨 파일 목록
}

// 4. [응답-하위] 개별 파일 정보
[Serializable]
public class EnvironmentFileDto
{
    public long fileId;
    public string fileType; // "SPACE" 또는 "MARKER"
    public string fileName;
    public string fileUrl;  // S3 다운로드 주소
}

// 5. [요청] 업로드 URL 요청
[Serializable]
public class S3PresignedUrlRequestDto
{
    public string fileName;
    public string fileType; // "SPACE" 또는 "MARKER"
}

// 6. [응답] 업로드 URL 정보
[Serializable]
public class S3PresignedUrlResponseDto
{
    public string presignedUploadUrl;
    public string finalFileUrl;
}


