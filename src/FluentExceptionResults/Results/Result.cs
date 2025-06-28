namespace FluentExceptionResults.Results;

public class Result
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<Error> Errors { get; set; } = new();

    public static Result Success(string? message = null) =>
        new() { IsSuccess = true, Message = message };

    public static Result Failure(string code, string message) =>
        new() { IsSuccess = false, Errors = { new Error { Code = code, Message = message } } };
}
public class Result<T>:Result
{
    public T? Value { get; set; }

    public static Result<T> Success(T value, string? message = null) =>
        new() { IsSuccess = true, Value = value, Message = message };

    public static Result<T> Failure(string code, string message,int statusCode) =>
        new() { IsSuccess = false, Errors = { new Error { Code = code, Message = message ,HttpStatus=statusCode } } };
}