namespace WebApp.DB.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public T Value { get; }
    protected Result(bool isSuccess, string error, T value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }
    public static Result<T> Success(T value) => new Result<T>(true, null, value);
    public static Result<T> Failure(string error) => new Result<T>(false, error, default);
}
