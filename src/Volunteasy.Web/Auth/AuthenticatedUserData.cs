using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Web.Auth;

public class SessionProvider : AuthenticationStateProvider, ISession
{

    private readonly IIdentityService _identity;

    private readonly FirebaseAuth _firebase;

    public SessionProvider(IIdentityService identity, FirebaseAuth firebase)
    {
        _identity = identity;
        _firebase = firebase;
    }
    
    private ClaimsPrincipal User { get; set; } = new ();
    
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(User));
    }

    public static (long, long) GetFromState(AuthenticationState state)
    {
        return (
            Convert.ToInt64(state.User.FindFirst("volunteasy_id")?.Value),
            Convert.ToInt64(state.User.FindFirst("organization_id")?.Value));
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
    
    public async Task AuthenticateUser(string email, string password)
    {

        try
        {
            var token = await _identity.AuthenticateUser(new UserCredentials
            {
                Email = email, Password = password
            });

            var claims = await _firebase.VerifyIdTokenAsync(token);
            if (claims == null)
                throw new Exception("no claim");
            
            var userId = Convert.ToInt64(claims.Claims["volunteasy_id"] ?? "volunteasy_id");
            
            User = new ClaimsPrincipal(
                new ClaimsIdentity(await _identity.GetUserSessionClaims2(userId), JwtBearerDefaults.AuthenticationScheme));

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(User)));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            User = new ClaimsPrincipal();
        }
        
    }
}