using ControleGastos.Api.DTOs;
using ControleGastos.Api.Exceptions;
namespace ControleGastos.Api.Middleware;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            var status = ex is AppException app ? app.StatusCode : 500;
            if (status == 500) logger.LogError(ex, "Erro não tratado");
            context.Response.StatusCode = status;
            // Em desenvolvimento, a causa real acelera o diagnóstico. Em produção,
            // detalhes internos continuam ocultos para não expor dados da aplicação.
            var mensagem = status == 500 && !environment.IsDevelopment()
                ? "Ocorreu um erro interno."
                : ex.Message;
            await context.Response.WriteAsJsonAsync(new ErroDto(status, mensagem));
        }
    }
}
