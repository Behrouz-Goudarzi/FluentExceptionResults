namespace FluentExceptionResults.Exceptions;

public abstract class AppException : Exception
{
    public ErrorEnumeration? Code { get; }
    public string? UserMessage { get; }
    public int HttpStatus { get; }
    public string? ApplicationName { get; }
    protected AppException(ErrorEnumeration? code=null, string? userMessage=null, int? httpStatus = null,string? applicationName=null, Exception? inner = null)
        : base(userMessage, inner)
    {
        Code = code;
        UserMessage = userMessage;
        HttpStatus = httpStatus ?? (this is BusinessException ? 200 : 400);
        ApplicationName = applicationName;
    }

    public override string ToString() => $"[{ApplicationName}] [{Code}] {UserMessage}";
}