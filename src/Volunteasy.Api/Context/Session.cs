using System.Security.Claims;
using Volunteasy.Core.Enums;
using static System.Enum;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Api.Context;

public class Session : ISession
{
    private readonly ClaimsPrincipal _user;

    public Session(IHttpContextAccessor context)
    {
        var ctx = context.HttpContext ?? new DefaultHttpContext();
        _user = ctx.User;
        
        UserId = Convert.ToInt64(ctx.User.FindFirst("volunteasy_id")?.Value);
        OrganizationId = Convert.ToInt64(ctx.Request.RouteValues["organizationId"]);
    }
    
    public Session(HttpContext ctx)
    {
        _user = ctx.User;
        UserId = Convert.ToInt64(ctx.User.FindFirst("volunteasy_id")?.Value);
        OrganizationId = Convert.ToInt64(ctx.Request.RouteValues["organizationId"]);
    }

    public long UserId { get; }
    
    public long OrganizationId { get; }
    
    public MembershipRole CurrentRole
    {
        get
        {
            var orgAuthentication = _user.Identities
                .Where(identity => identity.Name == OrganizationId.ToString())
                .Select(identity => identity.AuthenticationType ?? "")
                .SingleOrDefault();

            return string.IsNullOrEmpty(orgAuthentication) ? 0 : Parse<MembershipRole>(orgAuthentication);
        }
    }
}