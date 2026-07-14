using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace UserManagementApi.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("HTTP {Method} {Path} started", method, path);

        try
        {
            await next(context);
            stopwatch.Stop();
            logger.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMs} ms",
                method,
                path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await HandleExceptionAsync(context, ex, method, path, stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string? method,
        string? path,
        long elapsedMs)
    {
        var (statusCode, title, errors) = exception switch
        {
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "Ошибка валидации",
                (object?)validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message, null),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message, null),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message, null),
            _ => (HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера", null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(
                exception,
                "HTTP {Method} {Path} failed with {StatusCode} in {ElapsedMs} ms",
                method,
                path,
                (int)statusCode,
                elapsedMs);
        }
        else
        {
            logger.LogWarning(
                exception,
                "HTTP {Method} {Path} failed with {StatusCode} in {ElapsedMs} ms: {Message}",
                method,
                path,
                (int)statusCode,
                elapsedMs,
                exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new
        {
            status = (int)statusCode,
            title,
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
