using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Pages.Auth;
public class Register : PageModel
{
    private readonly IIdentityService _identity;

    private readonly IUserService _users;

    public Register(IIdentityService identity, IUserService users)
    {
        _identity = identity;
        _users = users;
    }

    public void OnGet()
    {
    }

    public async Task<ActionResult> OnPost([FromForm] UserRegistration credentials, [FromForm] Address address)
    {
        var user = await _users.CreateUser(credentials with { Address = address });

        var identity = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name!),
            new(ClaimTypes.Email, user.Email!)
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
        {
            IsPersistent = true
        });
        
        return Redirect("/");
    }
}