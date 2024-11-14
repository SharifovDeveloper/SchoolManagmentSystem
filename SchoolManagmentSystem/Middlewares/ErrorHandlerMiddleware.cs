using Domain.Exeptions;
using System.Net;
using System.Text.Json;

namespace Domain.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            EntityNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Error = exception is EntityNotFoundException ? "Not Found" : "Internal Server Error",
            Message = exception is EntityNotFoundException entityNotFoundEx
                ? entityNotFoundEx.Message
                : "Something went wrong, please try again later."
        };

        var result = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(result);

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
    }
}
