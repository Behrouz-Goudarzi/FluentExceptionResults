namespace FluentExceptionResults.Exceptions;

public abstract class BusinessException : AppException
{
    protected BusinessException(ErrorEnumeration code, string message)
        : base(code, message, 200)
    {
    }
}