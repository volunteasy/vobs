using Microsoft.AspNetCore.Components.Authorization;
using Volunteasy.Core.Enums;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Web.Auth;

public class SessionProvider : ISession
{

    private readonly AuthenticationStateProvider _provider;

    public SessionProvider(AuthenticationStateProvider provider)
    {
        _provider = provider;
    }

    public long UserId =>
        Convert.ToInt64(
            _provider.GetAuthenticationStateAsync().GetAwaiter().GetResult().User.FindFirst("volunteasy_id"));

    public long OrganizationId { get; } = 0;
    public MembershipRole CurrentRole { get; } = MembershipRole.Owner;
}