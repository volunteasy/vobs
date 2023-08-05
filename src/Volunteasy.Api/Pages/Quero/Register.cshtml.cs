using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Pages.Quero;

[BindProperties]
public class Register : OrganizationPageModel
{
    private readonly IBeneficiaryService _beneficiaries;

    public BeneficiaryCreation Creation { get; set; } = new()
    {
        Address = new()
    };
    
    public string? ErrorDesc { get; private set; } = "";

    public Register(IBeneficiaryService beneficiaryService, IOrganizationService organizationService) : base(organizationService)
    {
        _beneficiaries = beneficiaryService;
    }

    public async Task OnPost([FromQuery] string? returnUrl)
    {
        try
        {

            if (!ModelState.IsValid)
            {
                ErrorDesc = "Preencha as informações corretamente";
                return;
            }

            if (Creation.Address != null && Creation.Address.AddressName.IsNullOrEmpty())
            {
                Creation.Address = null;
            }
            
            var user = await _beneficiaries.CreateBeneficiary(Creation);

            var identity = new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email ?? "")
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = returnUrl ?? $"/quero/{OrganizationRouteSlug}"
            });
        }
        catch (Exception e)
        {
            if (e is not ApplicationException)
            {
                throw;
            }

            ErrorDesc = e.Message;
        }
    }
}