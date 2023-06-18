using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Core.DTOs;
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
    public async Task<IActionResult> CreateOrganization(OrganizationRegistration registration)
        => Created((await _organizations.CreateOrganization(registration)).Id.ToString(), null);

    [HttpGet("{organizationId:long}")]
    public async Task<IActionResult> GetOrganizationById(long organizationId)
        => Ok(await _organizations.GetOrganizationById(organizationId));
    
    [HttpGet]
    public async Task<IActionResult> ListOrganizations([FromQuery] OrganizationFilter filter)
    {
        
        var (organizations, next) = await _organizations.ListOrganizations(filter);
        return PaginatedList(organizations, next);
    }

    [HttpPut("{organizationId:long}")]
    [Authorize]
    public async Task<IActionResult> UpdateOrganizationById(long organizationId, OrganizationRegistration registration)
    {
        await _organizations.UpdateOrganizationById(organizationId, registration);
        return NoContent();
    }
}