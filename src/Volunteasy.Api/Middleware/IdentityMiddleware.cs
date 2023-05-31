using Volunteasy.Application;

namespace Volunteasy.Api.Middleware;

public class IdentityMiddleware
{
    private readonly IAuthenticator _auth;
    private readonly RequestDelegate _next;
    private readonly ILogger<IdentityMiddleware> _log;

    public IdentityMiddleware(RequestDelegate next, IAuthenticator auth, ILogger<IdentityMiddleware> log)
    {
        _next = next;
        _auth = auth;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var claims = await _auth.GetClaimsByToken(
                context.Request.Headers.Authorization
                    .FirstOrDefault()?
                    .Split("Bearer")
                    .Last()
                    .Trim() ?? ""
            );

            if (claims is not null)
                context.User = claims;
        }
        catch (Exception e)
        {
            _log.LogWarning("Could not get/parse token from request: {Exception}", e);
        }
        finally
        {
            await _next(context);
        }
    }
}