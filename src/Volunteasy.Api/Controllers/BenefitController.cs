using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Api.Context;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/organizations/{organizationId:long}/benefits")]
[Authorize]
public class BenefitController : BaseController
{
    private readonly IBenefitService _benefits;
    
    private readonly IBenefitProvisionService _benefitProvision;
    
    public BenefitController(IBenefitService benefits, IBenefitProvisionService benefitProvision)
    {
        _benefits = benefits;
        _benefitProvision = benefitProvision;
    }

    [HttpPost("provide")]
    [AuthorizeRoles(MembershipRole.Owner, MembershipRole.Volunteer)]
    public async Task<IActionResult> ProvideBenefit(BenefitProvision provision)
    {
        var res = await _benefitProvision.ProvideBenefit(provision);
        return Created(res.Id.ToString(), new { res.Position });
    }

    [HttpGet]
    [AuthorizeRoles]
    public async Task<IActionResult> ListBenefits([FromQuery] BenefitFilter filter, [FromQuery] long pageToken)
    {
        var (organizations, next) = await _benefits.ListBenefits(filter, pageToken);
        return PaginatedList(organizations, next);
    }
    
    [HttpGet("{benefitId:long}")]
    [AuthorizeRoles]
    public async Task<IActionResult> GetBenefitById(long benefitId)
        => Ok(await _benefits.GetBenefitById(benefitId));
    
    [HttpPost("{benefitId:long}/claim")]
    [AuthorizeRoles(MembershipRole.Owner, MembershipRole.Volunteer)]
    public async Task<IActionResult> ClaimBenefit(long benefitId)
    {
        await _benefits.ClaimBenefit(benefitId);
        return NoContent();
    }
}