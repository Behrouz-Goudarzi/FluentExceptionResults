using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentExceptionResults.Dependencies.Middleware;

public class CorrelationIdMiddleware : IMiddleware
{
    private const string HeaderKey = "X-Correlation-ID";
    public const string HttpContextKey = "CorrelationId";
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        context.Items[HttpContextKey] = correlationId;

        // اضافه کردن به Header برای پاسخ
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderKey] = correlationId;
            return Task.CompletedTask;
        });

        // استفاده در scope لاگ
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderKey, out var cid) && !string.IsNullOrWhiteSpace(cid))
        {
            return cid!;
        }

        return Guid.NewGuid().ToString();
    }
}
