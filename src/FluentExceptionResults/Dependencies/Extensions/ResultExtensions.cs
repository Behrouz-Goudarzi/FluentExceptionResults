using FluentExceptionResults.Exceptions;
using FluentExceptionResults.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FluentExceptionResults.Dependencies.Extensions;

public static class ResultExtensions
{

    private static Error FromException(AppException ex) =>
        new() { Code = ex.Code.ToString(), Message = ex.UserMessage ?? ex.Message, HttpStatus = ex.HttpStatus };

    private static Result<T> Failure<T>(AppException ex) =>
        new() { IsSuccess = false, Errors = { FromException(ex) }, Message = ex.UserMessage ?? ex.Message };
    public static async Task<Result<T>> ToResultAsync<T>(this Task<T> task, ILogger logger)
    {
        try
        {
            var result = await task;
            return Result<T>.Success(result);
        }
        catch (AppException ex)
        {
            logger.LogError(ex, "Handled Exception [{Code}]: {Message}", ex.Code, ex.UserMessage);
            return Failure<T>(ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
            return Result<T>.Failure("Unknown", "خطای ناشناخته، لطفا با پشتیبانی تماس بگیرید!", 500);
        }
    }

    public static Result<T> ToResult<T>(this Func<T> func, ILogger logger)
    {
        try
        {
            var result = func();
            return Result<T>.Success(result);
        }
        catch (AppException ex)
        {
            if(ex is BusinessException)
            logger.LogError(ex, "Handled Exception [{Code}]: {Message}", ex.Code, ex.Message);
            return Failure<T>(ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
            return Result<T>.Failure("Unknown", "خطای ناشناخته، لطفا با پشتیبانی تماس بگیرید!", 500);
        }
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(new { success = true, data = result.Value });

        var error = result.Errors.FirstOrDefault();
        var status = error?.HttpStatus;
        if (status == 400)
        {
            return new ObjectResult(new
            {
                success = result.IsSuccess,
                traceId = Activity.Current?.Id,
                code = "Validation.Failed",
                message = "خطاهای اعتبارسنجی رخ داده است.",
                errors = result.Errors
            })
            { StatusCode = status };
        }
        return new ObjectResult(new
        {
            success = result.IsSuccess,
            message = result.Message,
            traceId = Activity.Current?.Id,
            errors = result.Errors
        })
        { StatusCode = status };
    }
}