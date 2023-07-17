using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Pages.Auth;

public class Organization : PageModel
{
    private readonly IOrganizationService _organizations;

    public Organization(IOrganizationService organizations)
    {
        _organizations = organizations;
    }

    public void OnGet()
    {
    }

    public async Task<ActionResult> OnPost([FromForm] OrganizationRegistration credentials, [FromForm] Address address)
    {
        var organization = await _organizations.CreateOrganization(credentials with { Address = address });

        HttpContext.User.AddIdentity(new ClaimsIdentity(
            new List<Claim>
            {
                new(organization.Name!, "organization_name"),
                new(organization.Id.ToString(), "organization_id"),
            }));

        return Redirect("/");
    }
}