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

    public void OnGet()
    {
    }

    public async Task<ActionResult> OnPost([FromForm] UserCredentials credentials)
    {
        var user = await _identity.AuthenticateUser(credentials);

        var identities = user.Memberships.Select(m => new ClaimsIdentity(
                new List<Claim>
                {
                    new(m.OrganizationName!, "organization_name"),
                    new(m.OrganizationId.ToString(), "organization_id"),
                }))
            .Append(new ClaimsIdentity(new List<Claim>
            {
                new(user.Id.ToString(), ClaimTypes.NameIdentifier),
                new(user.Name, ClaimTypes.Name),
                new(user.Email, ClaimTypes.Email)
            }, CookieAuthenticationDefaults.AuthenticationScheme)).ToList();

        await HttpContext.SignInAsync(new ClaimsPrincipal(identities), new AuthenticationProperties
        {
            IsPersistent = true
        });
        
        return Redirect("/");
    }
}