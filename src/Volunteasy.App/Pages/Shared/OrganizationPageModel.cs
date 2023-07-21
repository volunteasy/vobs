using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.App.Pages.Shared;

public class OrganizationPageModel : PageModel
{
    protected readonly IOrganizationService OrganizationsService;
    
    public Organization? Organization { get; private set; }

    protected OrganizationPageModel(IOrganizationService organizations)
    {
        OrganizationsService = organizations;
    }

    protected string OrganizationRouteSlug
    {
        get {
            try
            {
                return HttpContext.GetRouteValue("orgSlug")!.ToString()!;
            }
            catch (Exception e)
            {
                throw new ArgumentNullException("Organization slug could not be retrieved", e);
            }
        }
    }

    public virtual Task<ActionResult> OnGet()
    {
        if (HttpContext.Items.TryGetValue("organization", out var org) && org != null)
            Organization = (Organization)org;

        return Task.FromResult<ActionResult>(Page());
    }
}