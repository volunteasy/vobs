using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Services;

namespace Volunteasy.App.Pages.Quero;

public class IndexModel : OrganizationPageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IOrganizationService organizations, ILogger<IndexModel> logger) : base(organizations)
    {
        _logger = logger;
    }

    public override async Task<ActionResult> OnGet()
    {
        if (!HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            //return Challenge();
            return RedirectToPage(("login"), new { orgSlug = OrganizationRouteSlug, ReturnUrl = HttpContext.Request.Path});
        }
        
        return await base.OnGet();
    }
}