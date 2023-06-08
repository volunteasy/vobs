using Volunteasy.Api.Response;

namespace Volunteasy.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception e)
        {
            var status = 0;
            if (e is not ApplicationException && !int.TryParse(e.HelpLink, out status))
            {
                _logger.LogError(e, "Captured unexpected error");
                await ctx.Response.WriteAsJsonAsync(new Response.Response
                {
                    Context = ctx,
                    Reason = "UnexpectedError",
                    Message = "Ops! Um erro inesperado ocorreu. Tente novamente mais tarde"
                });
            }

            _logger.LogWarning(e, "Captured expected error");
            ctx.Response.StatusCode = status;
            
            await ctx.Response.WriteAsJsonAsync(new Response.Response
            {
                Context = ctx,
                Reason = e.GetType().Name,
                Message = e.Message
            });
        }
    }
}