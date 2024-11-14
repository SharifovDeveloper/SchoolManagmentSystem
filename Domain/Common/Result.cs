namespace Domain.Common;

public class Result<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }

    public Result(T data)
    {
        Success = true;
        Data = data;
    }

    public Result(string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }
}
