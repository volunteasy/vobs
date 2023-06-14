using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Services;
using ISession = Volunteasy.Application.ISession;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Authorize]
[Route("/api/v1/organizations/{organizationId:long}/members")]
public class MembershipController : BaseController
{
    private readonly IOrganizationService _organizations;

    private readonly ISession _session;
    
    public MembershipController(IOrganizationService organizations, ISession session)
    {
        _organizations = organizations;
        _session = session;
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Enroll organization",
        Description = "Enrolls the current user in the organization specified by the parameterized id"
    )]
    public async Task<IActionResult> EnrollOrganization(long organizationId,
        [SwaggerParameter("Possible values are 'volunteer' and 'assisted'. Defaults to 'assisted'")]
        MembershipRole enrollAs)
    {
        await _organizations.CreateMembership(organizationId, _session.UserId, enrollAs);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> LeaveOrganization(long organizationId)
    {
        await _organizations.RemoveMembership(organizationId, _session.UserId);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> ListMemberships(long organizationId, int start, int end,
        DateTime? since, DateTime? until, MembershipRole? role, MembershipStatus? status)
    {
        var filter = new MembershipFilter
        {
            MemberSince = since, MemberUntil = until, Type = role,
            Status = status, OrganizationId = organizationId,
            ReadRange = (start, end)
        };
        
        var (members, hasNext) = await _organizations.ListMemberships(filter);
        return PaginatedList(members, filter, hasNext);
    }

    [HttpPut("{memberId:long}/role")]
    public async Task<IActionResult> ChangeMembershipRole(long organizationId, long memberId, MembershipRole role)
    {
        await _organizations.ChangeMembershipType(organizationId, memberId, role);
        return NoContent();
    }
    
    [HttpPut("{memberId:long}/status")]
    public async Task<IActionResult> ChangeMembershipStatus(long organizationId, long memberId, MembershipStatus status)
    {
        await _organizations.ChangeMembershipStatus(organizationId, memberId, status);
        return NoContent();
    }
}