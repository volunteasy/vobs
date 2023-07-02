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
    
    public DistributionController(IDistributionService distributions)
    {
        _distributions = distributions;
    }

    [HttpPost]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> CreateDistribution(DistributionCreationProperties props)
        => Created((await _distributions.CreateDistribution(props)).Id.ToString(), null);

    [HttpGet]
    public async Task<IActionResult> ListDistributions([FromQuery] DistributionFilter filter, [FromQuery] long pageToken)
    {
        var (organizations, next) = await _distributions.ListDistributions(filter, pageToken);
        return PaginatedList(organizations, next);
    }
    
    [HttpGet("{distributionId:long}")]
    public async Task<IActionResult> GetDistributionById(long distributionId)
        => Ok(await _distributions.GetDistributionById(distributionId));
    
    [HttpPut("{distributionId:long}")]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> UpdateDistribution(long distributionId, DistributionCreationProperties props)
    {
        await _distributions.UpdateDistribution(distributionId, props);
        return NoContent();
    }
    
    [HttpPost("{distributionId:long}/cancel")]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> CancelDistribution(long distributionId)
    {
        await _distributions.CancelDistribution(distributionId);
        return NoContent();
    }
    
    [HttpPost("{distributionId:long}/reopen")]
    [AuthorizeRoles(MembershipRole.Volunteer, MembershipRole.Owner)]
    public async Task<IActionResult> ReopenDistribution(long distributionId)
    {
        await _distributions.ReopenDistribution(distributionId);
        return NoContent();
    }
}