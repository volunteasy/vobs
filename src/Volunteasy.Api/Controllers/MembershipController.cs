using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Api.Context;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Authorize]
[Route("/api/v1/organizations/{organizationId:long}/members")]
public class MembershipController : BaseController
{
    private readonly IMembershipService _memberships;

    private readonly IVolunteasyContext _session;
    
    public MembershipController(IMembershipService memberships, IVolunteasyContext session)
    {
        _memberships = memberships;
        _session = session;
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Enroll organization",
        Description = "Enrolls the current user in the organization specified by the parameterized id"
    )]
    public async Task<IActionResult> EnrollOrganization(long organizationId, MembershipRole enrollAs)
    {
        await _memberships.EnrollOrganization(organizationId, _session.UserId, enrollAs);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> LeaveOrganization(long organizationId)
    {
        await _memberships.LeaveOrganization(organizationId, _session.UserId);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> ListMemberships([FromQuery] MembershipFilter filter, long organizationId, long pageToken)
    {
        var (members, next) = await _memberships.ListMemberships(
            filter with { OrganizationId = organizationId }, pageToken);
        return PaginatedList(members, next);
    }

    [HttpPut("{memberId:long}/role")]
    public async Task<IActionResult> ChangeMembershipRole(long organizationId, long memberId, MembershipRole role)
    {
        await _memberships.ChangeMembershipRole(organizationId, memberId, role);
        return NoContent();
    }
    
    [HttpPut("{memberId:long}/status")]
    [AuthorizeRoles(MembershipRole.Owner)]
    public async Task<IActionResult> ChangeMembershipStatus(long organizationId, long memberId, MembershipStatus status)
    {
        await _memberships.ChangeMembershipStatus(organizationId, memberId, status);
        return NoContent();
    }
}