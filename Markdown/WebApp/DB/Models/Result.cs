using WebApp.DB.Enums;

namespace WebApp.DB.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public Errors Error { get; }

    protected Result(bool isSuccess, string errorMessage, Errors error)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Error = error;
    }

    public static Result Success() => new Result(true, null, Errors.NotError);
    public static Result Failure(string errorMessage, Errors error = Errors.Unknown) => new Result(false, errorMessage, error);
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public Errors Error { get; }
    public T Value { get; }
    protected Result(bool isSuccess, string errorMessage, Errors error, T value)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Error = error;
        Value = value;
    }
    public static Result<T> Success(T value) => new Result<T>(true, null, Errors.NotError, value);
    public static Result<T> Failure(string errorMessage, Errors error = Errors.Unknown) => new Result<T>(false, errorMessage, error, default);
}
