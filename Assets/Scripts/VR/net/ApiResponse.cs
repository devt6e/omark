using System;

[Serializable]
public class T6ApiResponse<T>
{
    public string status;   // "OK" / "ERROR"
    public string message;  // optional
    public T data;
}
