using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.WebApp.Pages.Quero;

public class EnrollDistribution : OrganizationPageModel
{
    private readonly ILogger<EnrollDistribution> _logger;

    private readonly IDistributionService _distributions;

    private readonly IBenefitProvisionService _provision;

    private readonly IVolunteasyContext _ctx;

    public DistributionDto Distribution { get; private set; } = new();

    public Benefit Benefit { get; private set; } = new();

    public EnrollDistribution(IOrganizationService organizations, ILogger<EnrollDistribution> logger,
        IDistributionService distributions, IVolunteasyContext ctx, IBenefitProvisionService provision) : base(organizations)
    {
        _logger = logger;
        _distributions = distributions;
        _ctx = ctx;
        _provision = provision;
    }

    public async Task<ActionResult> OnPost([FromForm] long distributionId)
    {
        await base.OnGet();

        Distribution = await _distributions.GetDistributionById(distributionId);
        Benefit = await _provision.RequestBenefit(distributionId, new DistributionBenefitAnalysisRequest
        {
            Items = new List<BenefitDemandItem>
            {
                new ()
                {
                    Quantity = 1,
                }
            }
        });
        
        return Page();
    }
    
    
}