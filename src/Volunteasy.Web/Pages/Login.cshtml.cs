using System.Security.Claims;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class Login : PageModel
{

    private readonly IIdentityService _identity;

    private readonly FirebaseAuth _firebase;

    public Login(IIdentityService identity, FirebaseAuth firebase, IMembershipService memberships)
    {
        _identity = identity;
        _firebase = firebase;
    }
    
    public async Task<ActionResult> OnPost([FromForm] UserCredentials credentials)
    {
        var token = await _identity.AuthenticateUser(credentials);

        var claims = await _firebase.VerifyIdTokenAsync(token);
        if (claims == null)
            throw new Exception("no claim");

        var userId = Convert.ToInt64(claims.Claims["volunteasy_id"] ?? "volunteasy_id");

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(await _identity.GetUserSessionClaims2(userId),
                CookieAuthenticationDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user,
            new AuthenticationProperties
            {
                IsPersistent = true,
            });

        return Redirect("/login/org");
    }
}