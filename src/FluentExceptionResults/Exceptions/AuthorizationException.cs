namespace FluentExceptionResults.Exceptions;

public abstract class AuthorizationException: AppException
{
    protected AuthorizationException(ErrorEnumeration code, string message)
        : base(code, message, 403)
    {
    }
}