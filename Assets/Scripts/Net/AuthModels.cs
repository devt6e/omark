using System;
using System.Collections.Generic;

[Serializable]
public class SecurityQuestionResponseDto
{
    public long id;
    public string questionText;
}

[Serializable]
public class SignUpRequestDto
{
    public string email;
    public string password;
    public string name;
    public string phoneNumber;
    public long askId;
    public string askAnswer;
}

[Serializable]
public class LoginRequestDto
{
    public string email;
    public string password;
}

[Serializable]
public class LoginResponseDto
{
    public string id_token;
    public string access_token;
    public string refresh_token;
}

[Serializable]
public class RefreshTokenRequestDto
{
    public string email;
    public string refreshToken;
}

[Serializable]
public class RefreshTokenResponseDto
{
    public string id_token;
    public string access_token;
}

[Serializable]
public class FindEmailRequestDto
{
    public string phoneNumber;
    public long askId;
    public string askAnswer;
}

[Serializable]
public class FindEmailResponseDto
{
    public string email;
}

[Serializable]
public class ResetPasswordByQuestionRequestDto
{
    public string email;
    public string phoneNumber;
    public long askId;
    public string askAnswer;
    public string newPassword;
}

[Serializable]
public class CheckEmailRequestDto
{
    public string email;
}

[Serializable]
public class QuestionsResponse
{
    public string status;
    public string message;
    public List<SecurityQuestionResponseDto> data;
}

[Serializable]
public class LoginResponseWrapper
{
    public string status;
    public string message;
    public LoginResponseDto data;
}

[Serializable]
public class RefreshResponseWrapper
{
    public string status;
    public string message;
    public RefreshTokenResponseDto data;
}

[Serializable]
public class VoidResponse
{
    public string status;
    public string message;
}
