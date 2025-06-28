
using FluentExceptionResults.Dependencies.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FluentExceptionResults.Dependencies;
public static class FluentExceptionResultsRegistration
{
    public static IServiceCollection AddFluentExceptionResults(this IServiceCollection services)
    {
        services.AddExceptionHandler<AppExceptionHandler>();
        services.AddScoped<CorrelationIdMiddleware>();
        services.AddProblemDetails();

        return services;
    }

    public static IApplicationBuilder UseFluentExceptionResults(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseExceptionHandler();
        return app;
    }
}
