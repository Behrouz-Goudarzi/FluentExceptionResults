using FluentExceptionResults.Exceptions;
using FluentExceptionResults.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FluentExceptionResults.Dependencies.Middleware;

public sealed class AppExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AppExceptionHandler> _logger;

    public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        context.Response.ContentType = "application/json";
        var traceId = context.Items.TryGetValue(CorrelationIdMiddleware.HttpContextKey, out var cid)
    ? cid?.ToString()
    : context.TraceIdentifier;
        if (exception is AppException appEx)
        {
            if (appEx is ValidationException validEx)
            {
                context.Response.StatusCode = validEx.HttpStatus;
                var errorValidationResponse = new
                {
                    success = false,
                    traceId,
                    code = "Validation.Failed",
                    message = "خطاهای اعتبارسنجی رخ داده است.",
                    errors =  validEx.Errors
                };

                await context.Response.WriteAsJsonAsync(errorValidationResponse, cancellationToken);
                return true;

            }
            if (appEx is BusinessException)
                _logger.LogError(appEx.InnerException, "Handled Exception [{Code}] - {Message}", appEx.Code.Id, appEx.InnerException?.Message??appEx.Message);

            context.Response.StatusCode = appEx.HttpStatus;

            var errorResponse = new 
            {
                success = false,
                traceId=traceId,
                errors=new
                {
                    code = appEx.Code,
                    message = appEx.UserMessage
                }
            };

            await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }

        _logger.LogError(exception.InnerException??exception, "Unhandled Exception: {Message}", exception.InnerException?.Message??exception.Message);

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {

            success = false,
            traceId = traceId,
            errors=new{
                code = "ERR-Unknown",
                message = "خطای نا مشخص، با پشتیبانی تماس بگیرید!"
            }
            
        }, cancellationToken);

        return true;
    }
}