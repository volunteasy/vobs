using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/{organizationId:long}/distributions")]
[Authorize]
public class DistributionController : BaseController
{
    private readonly IDistributionService _distributions;
    
    public DistributionController(IDistributionService distributions)
    {
        _distributions = distributions;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDistribution(DistributionCreationProperties props)
        => Created((await _distributions.CreateDistribution(props)).Id.ToString(), null);

    [HttpGet]
    public async Task<IActionResult> ListDistributions([FromQuery] DistributionFilter filter, [FromQuery] long pageToken)
    {
        var (organizations, next) = await _distributions.ListDistributions(filter, pageToken);
        return PaginatedList(organizations, next);
    }
    
    [HttpGet("{benefitId:long}")]
    public async Task<IActionResult> GetDistributionById(long benefitId)
        => Ok(await _distributions.GetDistributionById(benefitId));
    
    [HttpPut("{benefitId:long}")]
    public async Task<IActionResult> UpdateDistribution(long benefitId, DistributionCreationProperties props)
    {
        await _distributions.UpdateDistribution(benefitId, props);
        return NoContent();
    }
    
    [HttpPost("{benefitId:long}/cancel")]
    public async Task<IActionResult> CancelDistribution(long benefitId)
    {
        await _distributions.CancelDistribution(benefitId);
        return NoContent();
    }
    
    [HttpPost("{benefitId:long}/reopen")]
    public async Task<IActionResult> ReopenDistribution(long benefitId)
    {
        await _distributions.ReopenDistribution(benefitId);
        return NoContent();
    }
}