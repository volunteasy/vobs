using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;
using Volunteasy.Infrastructure.Firebase;

namespace Volunteasy.WebApp.Pages.Quero;
public class Login : OrganizationPageModel
{
    private readonly IBeneficiaryService _identity;

    public string ErrorDesc { get; private set; } = "";

    public Login(IBeneficiaryService identity, IOrganizationService organizationService) : base(organizationService)
    {
        _identity = identity;
    }

    public async Task OnPost([FromForm] BeneficiaryKey credentials, [FromQuery] string? returnUrl)
    {
        try
        {
            var user = await _identity.GetBeneficiaryByDocumentAndBirthDate(credentials);
            var identity = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email ?? "")
            }, CookieAuthenticationDefaults.AuthenticationScheme));
        
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = returnUrl ?? $"/quero/{OrganizationRouteSlug}",
                Parameters = {  }
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