using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Services;

namespace Volunteasy.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class OrganizationLogin : PageModel
{
    public bool IsLoggedIn => HttpContext.User.Identity?.IsAuthenticated ?? false;

    public List<OrganizationMember>? Organizations;

    private readonly IMembershipService _memberships;

    public OrganizationLogin(IMembershipService memberships)
    {
        _memberships = memberships;
    }

    public async Task OnGet()
    {
        var userId = Convert.ToInt64(HttpContext.User.FindFirst("volunteasy_id")?.Value ?? "");
        var page = (long)0;

        do
        {
            var (organizations, next) = await _memberships.ListMemberships(new MembershipFilter
            {
                MemberId = userId, Role = MembershipRole.Owner
            }, page);

            if (next == null)
                break;

            page = Convert.ToInt64(next);

            if (Organizations == null)
            {
                Organizations = organizations.ToList();
                continue;
            }

            Organizations.AddRange(organizations);
        } while (page == 0);
    }
    
    public async Task OnPost([FromForm] long organizationId)
    {
        var user = HttpContext.User.Identity;
        if (user == null)
            throw new ApplicationException();

        if (!user.IsAuthenticated)
            throw new UserNotAuthorizedException();

        var userId = Convert.ToInt64(HttpContext.User.FindFirst("volunteasy_id")?.Value ?? "");
        var (res, _) = await _memberships.ListMemberships(new MembershipFilter
        {
            MemberId = userId,
            OrganizationId = organizationId,
        }, 0);

        if (!res.Any())
            throw new Exception();

        var identity = new ClaimsPrincipal(
            new ClaimsIdentity(User.Claims
                    .Where(c => c.Type != "organization_id")
                    .Append(new Claim("organization_id", organizationId.ToString()))
                , JwtBearerDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
            {
                IsPersistent = true,
            });
    }
}