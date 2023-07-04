using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Web.Auth;

public class AuthenticatedUserData : AuthenticationStateProvider
{

    private readonly IIdentityService _identity;

    private readonly FirebaseAuth _firebase;

    public AuthenticatedUserData(IIdentityService identity, FirebaseAuth firebase)
    {
        _identity = identity;
        _firebase = firebase;
    }
    
    private ClaimsPrincipal User { get; set; } = new ();
    
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(User));
    }
    
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
                return;
            
            var userId = Convert.ToInt64(claims.Claims["volunteasy_id"] ?? "volunteasy_id");
            
            var identity = new ClaimsIdentity(await _identity.GetUserSessionClaims2(userId), JwtBearerDefaults.AuthenticationScheme);
            
            User = new ClaimsPrincipal((await _identity.GetUserSessionClaims(userId)).Append(identity));

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(User)));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            User = new ClaimsPrincipal();
        }
        
    }

    public async Task SetOrganizationAs(long orgId)
    {
        User.Identity

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(User)));
    }
}