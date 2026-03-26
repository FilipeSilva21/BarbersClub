using System.Net;
using System.Text.Json;
using Business.Error_Handling;

namespace Web.Middleware;

public class ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger, IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "Um erro ocorreu: {Message}", exception.Message);

        var statusCode = HttpStatusCode.InternalServerError; // 500 por padrão
        var message = "Ocorreu um erro interno inesperado.";
        object? details;

        switch (exception)
        {
            case { } ex when ex is UserIdNotFoundException ||
                             ex is EmailNotFoundException ||
                             ex is ServiceNotFoundException ||
                             ex is BarberShopNotFoundException ||
                             ex is RatingNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = ex.Message;
                break;

            case { } ex when ex is DuplicateEmailException ||
                             ex is ServiceAlreadyScheduledException ||
                             ex is RatingAlreadyExistException:
                statusCode = HttpStatusCode.Conflict;
                message = ex.Message;
                break;
            
            case InvalidCredentialsException ex:
                statusCode = HttpStatusCode.Unauthorized;
                message = ex.Message;
                break;

            case ArgumentException ex:
                statusCode = HttpStatusCode.BadRequest;
                message = ex.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        
        if (env.IsDevelopment())
        {
            details = new { ErrorType = exception.GetType().Name, StackTrace = exception.ToString() };
        }
        else
        {
            details = new { ErrorType = exception.GetType().Name, StackTrace = "Internal Server Error" };
        }

        var errorResponse = new { statusCode = context.Response.StatusCode, message, details };
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        
        await context.Response.WriteAsync(jsonResponse);
    }
}