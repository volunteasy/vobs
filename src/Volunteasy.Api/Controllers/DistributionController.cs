using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Api.Context;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/organizations/{organizationId:long}/distributions")]
[Authorize]
public class DistributionController : BaseController
{
    private readonly IDistributionService _distributions;

    private readonly IBenefitService _benefits;
    
    private readonly IBenefitProvisionService _provision;
    
    public DistributionController(IDistributionService distributions, IBenefitService benefits, IBenefitProvisionService provision)
    {
        _distributions = distributions;
        _benefits = benefits;
        _provision = provision;
    }

    [HttpPost]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> CreateDistribution(DistributionCreationProperties props)
        => Created((await _distributions.CreateDistribution(props)).Id.ToString(), null);

    [HttpGet]
    [AuthorizeRoles]
    public async Task<IActionResult> ListDistributions([FromQuery] DistributionFilter filter, [FromQuery] long pageToken)
    {
        var (organizations, next) = await _distributions.ListDistributions(filter, pageToken);
        return PaginatedList(organizations, next);
    }
    
    [HttpGet("{distributionId:long}")]
    [AuthorizeRoles]
    public async Task<IActionResult> GetDistributionById(long distributionId)
        => Ok(await _distributions.GetDistributionById(distributionId));
    
    [HttpPut("{distributionId:long}")]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> UpdateDistribution(long distributionId, DistributionCreationProperties props)
    {
        await _distributions.UpdateDistribution(distributionId, props);
        return NoContent();
    }
    
    [HttpGet("{distributionId:long}/queue")]
    [AuthorizeRoles(MembershipRole.Owner, MembershipRole.Volunteer)]
    public async Task<IActionResult> ListQueue(long distributionId)
    {
        var (organizations, next) = await _benefits.ListBenefits(new BenefitFilter
        {
            DistributionId = distributionId, 
            NotClaimedOnly = true
        }, 0);
        
        return PaginatedList(organizations, next);
    }
    
    [HttpPost("{distributionId:long}/cancel")]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> CancelDistribution(long distributionId)
    {
        await _distributions.CancelDistribution(distributionId);
        return NoContent();
    }
    
    [HttpPost("{distributionId:long}/open")]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> OpenDistribution(long distributionId)
    {
        await _distributions.OpenDistribution(distributionId);
        return NoContent();
    }
    
    [HttpPost("{distributionId:long}/join")]
    [AuthorizeRoles(MembershipRole.Assisted)]
    public async Task<IActionResult> RequestBenefit(DistributionBenefitAnalysisRequest analysisRequest, long distributionId)
    {
        var res = await _provision.RequestBenefit(distributionId, analysisRequest);
        return Created(res.Id.ToString(), new { res.Position });
    }
}