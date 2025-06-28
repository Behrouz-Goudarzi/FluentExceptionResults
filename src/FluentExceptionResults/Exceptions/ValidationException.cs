
namespace FluentExceptionResults.Exceptions;

public sealed class ValidationException : AppException
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IReadOnlyList<ValidationError> errors):base(httpStatus:400)
    {
        Errors = errors;
    }
}
public sealed class ValidationError
{
    public string? PropertyName { get; init; } = default;
    public string? Message { get; init; }=default;

}