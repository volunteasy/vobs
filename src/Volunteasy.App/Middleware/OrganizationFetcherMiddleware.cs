using System.Security.Claims;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Services;

namespace Volunteasy.App.Middleware;

public class OrganizationSlugMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<OrganizationSlugMiddleware> _logger;

    public OrganizationSlugMiddleware(RequestDelegate next, ILogger<OrganizationSlugMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext ctx, IOrganizationService organizationService)
    {
        var slug = ctx.GetRouteValue("orgSlug")?.ToString() ?? "";
        try
        {
            var org = await organizationService.GetOrganizationBySlug(slug);
            ctx.User.AddIdentity(new ClaimsIdentity());
            ctx.Items.Add("organization", org);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Could not fetch organization with slug {Slug}", slug);
            throw;
        }
        
        await _next(ctx);
    }
}