using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.App.Pages.Quero;
public class Login : OrganizationPageModel
{
    private readonly IBeneficiaryService _identity;

    public Login(IBeneficiaryService identity, IOrganizationService organizationService) : base(organizationService)
    {
        _identity = identity;
    }

    public async Task OnPost([FromForm] BeneficiaryKey credentials)
    {
        var user = await _identity.GetBeneficiaryByDocumentAndBirthDate(credentials);

        var identity = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        }, CookieAuthenticationDefaults.AuthenticationScheme));
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
        {
            IsPersistent = true,
            Parameters = {  }
        });
    }
}