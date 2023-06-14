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
            
            if (e is ApplicationException && int.TryParse(e.HelpLink, out var status))
            {
                _logger.LogWarning("Captured expected error: {Message}, {Type}", e.Message, e.GetType().Name);
                ctx.Response.StatusCode = status;
            
                await ctx.Response.WriteAsJsonAsync(new Response.Response
                {
                    Context = ctx,
                    Reason = e.GetType().Name,
                    Message = e.Message
                });
                
                return;
            }

            _logger.LogError(e, "Captured unexpected error");
            ctx.Response.StatusCode = 500;
                
            await ctx.Response.WriteAsJsonAsync(new Response.Response
            {
                Context = ctx,
                Reason = "UnexpectedError",
                Message = "Ops! Um erro inesperado ocorreu. Tente novamente mais tarde"
            });
        }
    }
}