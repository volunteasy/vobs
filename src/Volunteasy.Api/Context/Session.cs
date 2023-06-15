using System.Security.Claims;
using Volunteasy.Core.Enums;
using static System.Enum;
using ISession = Volunteasy.Application.ISession;

namespace Volunteasy.Api.Context;

public class Session : ISession
{
    private readonly HttpContext _context;

    public Session(IHttpContextAccessor context)
    {
        _context = context.HttpContext ?? new DefaultHttpContext();
    }

    public long UserId => Convert.ToInt64(User.FindFirst("volunteasy_id")?.Value);

    private ClaimsPrincipal User => _context.User;

    public MembershipRole GetMembershipRole(long organizationId)
    {
        var orgAuthentication = User.Identities
            .Where(identity => identity.Name == organizationId.ToString())
            .Select(identity => identity.AuthenticationType ?? "")
            .SingleOrDefault();

        return string.IsNullOrEmpty(orgAuthentication) ? 0 : Parse<MembershipRole>(orgAuthentication);
    }
}