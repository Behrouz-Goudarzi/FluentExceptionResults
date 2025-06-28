namespace FluentExceptionResults.Exceptions;

public abstract class AuthenticationException : AppException
{
    protected AuthenticationException(ErrorEnumeration code, string message)
        : base(code, message, 401)
    {
    }
}