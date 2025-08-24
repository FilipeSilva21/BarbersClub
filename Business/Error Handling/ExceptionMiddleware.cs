using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Business.Error_Handling;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro capturado no middleware global");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status = HttpStatusCode.InternalServerError;
        string message = "Ocorreu um erro inesperado.";

        switch (exception)
        {
            case UserIdNotFoundException:
            case EmailNotFoundException:
            case ServiceNotFoundException:
            case BarberShopNotFoundException:
            case RatingNotFoundException:
                status = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case DuplicateEmailException:
            case ArgumentException:
                status = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case ServiceAlreadyScheduledException:
                status = HttpStatusCode.Conflict;
                message = exception.Message;
                break;
        }

        var result = JsonSerializer.Serialize(new
        {
            status = (int)status,
            error = message
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsync(result);
    }
}