using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/organizations")]
public class OrganizationController : BaseController
{
    private readonly IOrganizationService _organizations;
    
    public OrganizationController(IOrganizationService organizations)
    {
        _organizations = organizations;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrganization(Organization registration)
        => Created((await _organizations.CreateOrganization(registration)).Id.ToString(), null);

    [HttpGet("{organizationId:long}")]
    public async Task<IActionResult> GetOrganizationById(long organizationId)
        => Ok(await _organizations.GetOrganizationById(organizationId));
    
    [HttpGet]
    public async Task<IActionResult> ListOrganizations(string? name, int start, int end)
    {
        var filter = new OrganizationFilter
        {
            Name = name,
            ReadRange = (start, end)
        };
        
        var (organizations, hasNext) = await _organizations.ListOrganizations(filter);
        return PaginatedList(organizations, filter, hasNext);
    }

    [HttpPut("{organizationId:long}")]
    [Authorize]
    public async Task<IActionResult> UpdateOrganizationById(long organizationId, Organization registration)
    {
        await _organizations.UpdateOrganizationById(organizationId, registration);
        return NoContent();
    }
}