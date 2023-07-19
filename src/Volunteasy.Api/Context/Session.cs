using Volunteasy.Core.Enums;
using Volunteasy.Core.Services;
using static System.Enum;

namespace Volunteasy.Api.Context;

public class VolunteasyContext : IVolunteasyContext
{
    private readonly HttpContext? _context;

    public VolunteasyContext(IHttpContextAccessor context)
    {
        _context = context.HttpContext;
    }
    
    public VolunteasyContext(HttpContext ctx)
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