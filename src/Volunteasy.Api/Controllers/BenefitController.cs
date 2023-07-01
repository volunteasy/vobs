using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/{organizationId:long}/benefits")]
[Authorize]
public class BenefitController : BaseController
{
    private readonly IBenefitService _benefits;
    
    public BenefitController(IBenefitService benefits)
    {
        _benefits = benefits;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBenefit(BenefitDemand demand)
        => Created((await _benefits.CreateBenefit(demand)).Id.ToString(), null);

    [HttpGet]
    public async Task<IActionResult> ListBenefits([FromQuery] BenefitFilter filter, [FromQuery] long pageToken)
    {
        var (organizations, next) = await _benefits.ListBenefits(filter, pageToken);
        return PaginatedList(organizations, next);
    }
    
    [HttpGet("{benefitId:long}")]
    public async Task<IActionResult> GetBenefitById(long benefitId)
        => Ok(await _benefits.GetBenefitById(benefitId));
    
    [HttpPost("{benefitId:long}/claim")]
    public async Task<IActionResult> ClaimBenefit(long benefitId)
    {
        await _benefits.ClaimBenefit(benefitId);
        return NoContent();
    }
    
    [HttpPost("{benefitId:long}/cancel")]
    public async Task<IActionResult> CancelBenefit(long benefitId)
    {
        await _benefits.CancelBenefit(benefitId);
        return NoContent();
    }
}