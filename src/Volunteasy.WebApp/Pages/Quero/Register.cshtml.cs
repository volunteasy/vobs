using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.WebApp.Pages.Quero;
public class Register : OrganizationPageModel
{
    private readonly IBeneficiaryService _beneficiaries;

    public Register(IBeneficiaryService beneficiaryService, IOrganizationService organizationService) : base(organizationService)
    {
        _beneficiaries = beneficiaryService;
    }

    public async Task<ActionResult> OnPost([FromForm] BeneficiaryCreation credentials, [FromForm] Address address, [FromQuery] string returnUrl)
    {
        var user = await _beneficiaries.CreateBeneficiary(credentials with { Address = address });

        var identity = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        return SignIn(new ClaimsPrincipal(identity), new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = returnUrl
        }, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}