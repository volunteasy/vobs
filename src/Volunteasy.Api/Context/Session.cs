using Volunteasy.Core.Enums;
using static System.Enum;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Api.Context;

public class Session : ISession
{
    private readonly HttpContext? _context;

    public Session(IHttpContextAccessor context)
    {
        _context = context.HttpContext;
    }
    
    public Session(HttpContext ctx)
    {
        _context = ctx;
    }

    public long UserId => Convert.ToInt64(_context?.User.FindFirst("volunteasy_id")?.Value);
    
    public long OrganizationId => Convert.ToInt64(_context?.Request.RouteValues["organizationId"]);
    
    public MembershipRole CurrentRole
    {
        get
        {
            var orgAuthentication = _context?.User.Identities
                .Where(identity => identity.Name == OrganizationId.ToString())
                .Select(identity => identity.AuthenticationType ?? "")
                .SingleOrDefault();

            return string.IsNullOrEmpty(orgAuthentication) ? 0 : Parse<MembershipRole>(orgAuthentication);
        }
    }
}