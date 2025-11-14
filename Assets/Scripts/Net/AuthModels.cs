using System;
using System.Collections.Generic;

[Serializable] public class ApiWrap<T> { public string status; public string message; public T data; }

[Serializable] public class LoginReq { public string email; public string password; }
[Serializable] public class LoginRes { public string access_token; public string refresh_token; }

[Serializable] public class RefreshReq { public string email; public string refresh_token; }
[Serializable] public class RefreshRes { public string access_token; }

[Serializable] public class LogoutReq { public string email; }

[Serializable] public class Question { public int id; public string questionText; }
[Serializable] public class QuestionsRes { public List<Question> data; }

[Serializable] public class SignupReq { public string email; public string password; public string name; public string phone; public int questionId; public string answer; }

[Serializable] public class FindEmailReq { public string phone; public int questionId; public string answer; }
[Serializable] public class FindEmailRes { public string email; }

[Serializable] public class ResetPwByQuestionReq { public string email; public int questionId; public string answer; public string newPassword; }
