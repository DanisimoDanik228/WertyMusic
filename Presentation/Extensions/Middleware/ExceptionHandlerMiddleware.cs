using System.Net;
using System.Text.Json;

namespace WertyMusic.Extensions;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Invalid operation"),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Not implemented"),
            _ => (HttpStatusCode.InternalServerError, "Internal server error")
        };

        response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Detailed = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        var result = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(result);
    }
}