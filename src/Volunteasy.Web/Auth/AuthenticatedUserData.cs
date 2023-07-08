using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Volunteasy.Core.Enums;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Web.Auth;

public class SessionProvider : AuthenticationStateProvider, ISession
{
    private ClaimsPrincipal User { get; set; } = new ();
    
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(User));
    }

    public long UserId => Convert.ToInt64(User.FindFirst("volunteasy_id")?.Value);

    public long OrganizationId
    {
        get => Convert.ToInt64(User.FindFirst("organization_id")?.Value);
        set 
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(User.Claims
                        .Where(c => c.Type != "organization_id")
                        .Append(new Claim("organization_id", value.ToString()))
                    , JwtBearerDefaults.AuthenticationScheme));

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(User)));
        }
    }

    public MembershipRole CurrentRole => MembershipRole.Owner;
}