using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.WebApp.Pages.Quero;
public class Register : OrganizationPageModel
{
    private readonly IBeneficiaryService _beneficiaries;
    
    public string ErrorDesc { get; private set; } = "";

    public Register(IBeneficiaryService beneficiaryService, IOrganizationService organizationService) : base(organizationService)
    {
        _beneficiaries = beneficiaryService;
    }

    public async Task OnPost([FromForm] BeneficiaryCreation credentials, [FromForm] Address? address, [FromQuery] string? returnUrl)
    {
        try
        {
            if (address != null &&
                !string.IsNullOrEmpty(address.AddressName) &&
                !string.IsNullOrEmpty(address.AddressNumber))
            {
                credentials = credentials with { Address = address };
            }
            
            var user = await _beneficiaries.CreateBeneficiary(credentials);

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