using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Api.Context;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Services;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/organizations")]
[Authorize]
public class OrganizationController : BaseController
{
    private readonly IOrganizationService _organizations;

    private readonly IMembershipService _memberships;

    private readonly ISession _session;
    
    public OrganizationController(IOrganizationService organizations, IMembershipService memberships, ISession session)
    {
        _organizations = organizations;
        _memberships = memberships;
        _session = session;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(OrganizationRegistration registration)
        => Created((await _organizations.CreateOrganization(registration)).Id.ToString(), null);
    

    [HttpGet]
    public async Task<IActionResult> ListOrganizations([FromQuery] OrganizationFilter filter,
        [FromQuery] long pageToken)
    {
        var (organizations, next) = await _organizations
            .ListOrganizations(filter, pageToken);
        
        return PaginatedList(organizations, next);
    }
    
    [HttpGet("{organizationId:long}")]
    public async Task<IActionResult> GetOrganizationById(long organizationId)
        => Ok(await _organizations.GetOrganizationById(organizationId));

    [HttpPut("{organizationId:long}")]
    [AuthorizeRoles(MembershipRole.Owner)]
    public async Task<IActionResult> UpdateOrganizationById(long organizationId, OrganizationRegistration registration)
    {
        await _organizations.UpdateOrganizationById(organizationId, registration);
        return NoContent();
    }
    
    [HttpPost("{organizationId:long}/enroll")]
    [SwaggerOperation(
        Summary = "Enroll organization",
        Description = "Enrolls the current user in the organization specified by the parameterized id"
    )]
    public async Task<IActionResult> EnrollOrganization(long organizationId, [FromQuery] MembershipRole enrollAs)
    {
        await _memberships.EnrollOrganization(organizationId, _session.UserId, enrollAs);
        return NoContent();
    }
}