
using System.Security.Claims;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Web.Controllers;

[Route("/admin/auth")]
[ApiController]
public class AuthController : Controller
{
    
    private readonly IIdentityService _identity;
    
    private readonly IMembershipService _memberships;

    private readonly ISession _session;

    private readonly FirebaseAuth _firebase;

    public AuthController(IIdentityService identity, FirebaseAuth firebase, IMembershipService memberships, ISession session)
    {
        _identity = identity;
        _firebase = firebase;
        _memberships = memberships;
        _session = session;
    }
    
    [HttpPost]
    public async Task<ActionResult> Login([FromForm] UserCredentials credentials)
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

        return Redirect("/orglogin");
    }
    
    [HttpPost("org")]
    public async Task<ActionResult> OrgLogin([FromForm] long organizationId)
    {
        var (res, _) = await _memberships.ListMemberships(new MembershipFilter
        {
            MemberId = _session.UserId,
            OrganizationId = organizationId,
        }, 0);

        if (!res.Any())
        {
            throw new Exception();
        }
        
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

        return Redirect("/");
    }
    
    [HttpGet]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            IsPersistent = true
        });

        return Redirect("/");
    }
}