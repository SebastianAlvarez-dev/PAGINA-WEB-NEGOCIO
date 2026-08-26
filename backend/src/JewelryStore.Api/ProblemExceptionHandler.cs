using JewelryStore.BuildingBlocks.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JewelryStore.Api;

public sealed class ProblemExceptionHandler(
    IProblemDetailsService problemDetails,
    ILogger<ProblemExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "No pudimos completar la operación. Inténtalo nuevamente."
                    : exception.Message
            },
            Exception = exception
        });
    }
}

