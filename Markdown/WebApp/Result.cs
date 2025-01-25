namespace WebApp;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success() => new Result(true, null);
    public static Result Failure(string error) => new Result(false, error);
}
