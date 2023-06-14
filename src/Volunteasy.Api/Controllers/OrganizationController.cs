using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;
using ISession = Volunteasy.Application.ISession;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Authorize]
[Route("/api/v1/organizations")]
public class OrganizationController : BaseController
{
    private readonly IOrganizationService _organizations;
    
    public OrganizationController(IOrganizationService organizations)
    {
        _organizations = organizations;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(Organization registration)
        => Created((await _organizations.CreateOrganization(registration)).Id.ToString(), null);

    [HttpGet("{organizationId:long}")]
    public async Task<IActionResult> GetOrganizationById(long organizationId)
        => Ok(await _organizations.GetOrganizationById(organizationId));

    [HttpPut("{organizationId:long}")]
    public async Task<IActionResult> UpdateOrganizationById(long organizationId, Organization registration)
    {
        await _organizations.UpdateOrganizationById(organizationId, registration);
        return NoContent();
    }
}