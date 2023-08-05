using Volunteasy.Core.Errors;

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
        if (ctx.Request.Path.StartsWithSegments("/quero"))
        {
            await _next(ctx);
            return;
        }
        
        try
        {
            await _next(ctx);
        }
        catch (ResourceNotFoundException e)
        {
            ctx.Response.StatusCode = 404;
            await CaptureExpectedException(e, ctx);
        }
        catch (Exception e)
        {
            
            if (e is ApplicationException && int.TryParse(e.HelpLink, out var status))
            {
                ctx.Response.StatusCode = status;
                await CaptureExpectedException(e, ctx);
                return;
            }

            await CaptureUnexpectedException(e, ctx);
        }
    }

    private async Task CaptureExpectedException(Exception e, HttpContext context)
    {
        _logger.LogWarning("Captured expected error: {Message}, {Type}", e.Message, e.GetType().Name);
            
        await context.Response.WriteAsJsonAsync(new Response.Response
        {
            Context = context,
            Reason = e.GetType().Name,
            Message = e.Message
        });
    }
    
    private async Task CaptureUnexpectedException(Exception e, HttpContext context)
    {
        _logger.LogError(e, "Captured unexpected error");
        context.Response.StatusCode = 500;
            
        await context.Response.WriteAsJsonAsync(new Response.Response
        {
            Context = context,
            Reason = "UnexpectedError",
            Message = "Ops! Um erro inesperado ocorreu. Tente novamente mais tarde"
        });
    }
}