using System.Diagnostics;
using System.Security.Claims;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class Login : PageModel
{
    public bool IsLoggedIn => HttpContext.User.Identity?.IsAuthenticated ?? false;

    private readonly ILogger<Login> _logger;

    private readonly IIdentityService _identity;

    private readonly IMembershipService _memberships;

    private readonly FirebaseAuth _firebase;

    public Login(ILogger<Login> logger, IIdentityService identity, FirebaseAuth firebase, IMembershipService memberships)
    {
        _logger = logger;
        _identity = identity;
        _firebase = firebase;
        _memberships = memberships;
    }

    public async Task OnPost([FromForm] UserCredentials credentials)
    {
        var token = await _identity.AuthenticateUser(credentials);

        var claims = await _firebase.VerifyIdTokenAsync(token);
        if (claims == null)
            throw new Exception("no claim");
            
        var userId = Convert.ToInt64(claims.Claims["volunteasy_id"] ?? "volunteasy_id");

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(await _identity.GetUserSessionClaims2(userId),
                CookieAuthenticationDefaults.AuthenticationScheme));
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user, new AuthenticationProperties
        {
            IsPersistent = true,
        });
    }
    
    public async Task OnPost([FromForm] long organizationId)
    {
        var user = HttpContext.User.Identity;
        if (user == null)
            throw new ApplicationException();

        if (!user.IsAuthenticated)
            throw new UserNotAuthorizedException();

        var userId = Convert.ToInt64(HttpContext.User.FindFirst("volunteasy_id")?.Value ?? "");
        var (res, _) = await _memberships.ListMemberships(new MembershipFilter
        {
            MemberId = userId,
            OrganizationId = organizationId,
        }, 0);

        if (!res.Any())
            throw new Exception();

        var identity = new ClaimsPrincipal(
            new ClaimsIdentity(User.Claims
                    .Where(c => c.Type != "organization_id")
                    .Append(new Claim("organization_id", organizationId.ToString()))
                , JwtBearerDefaults.AuthenticationScheme));
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
            {
                IsPersistent = true,
            });
    }
}