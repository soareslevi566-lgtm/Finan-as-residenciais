using ControleGastos.Api.DTOs;
using ControleGastos.Api.Exceptions;
namespace ControleGastos.Api.Middleware;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            var status = ex is AppException app ? app.StatusCode : 500;
            if (status == 500) logger.LogError(ex, "Erro não tratado");
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ErroDto(status, status == 500 ? "Ocorreu um erro interno." : ex.Message));
        }
    }
}
