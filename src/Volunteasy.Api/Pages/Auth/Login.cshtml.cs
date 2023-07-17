using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Pages.Auth;
public class Login : PageModel
{
    private readonly IIdentityService _identity;

    public Login(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task<ActionResult> OnPost([FromForm] UserCredentials credentials)
    {
        var user = await _identity.AuthenticateUser(credentials);

        var identity = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        }, CookieAuthenticationDefaults.AuthenticationScheme));
        
        identity.AddIdentities(user.Memberships.Select(m => new ClaimsIdentity(
            new List<Claim>
            {
                new("organization_name", m.OrganizationName!),
                new("organization_id", m.OrganizationId.ToString()),
            })).ToList());

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
        {
            IsPersistent = true
        });
        
        return Redirect("/");
    }
}