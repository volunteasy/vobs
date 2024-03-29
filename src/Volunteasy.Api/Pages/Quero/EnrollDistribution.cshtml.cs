﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Pages.Quero;

public class EnrollDistribution : OrganizationPageModel
{
    private readonly ILogger<EnrollDistribution> _logger;

    private readonly IDistributionService _distributions;

    private readonly IBenefitProvisionService _provision;

    private readonly IVolunteasyContext _ctx;

    public DistributionDto Distribution { get; private set; } = new();

    public Benefit Benefit { get; private set; } = new();
    
    public string? BenefitRefusalReason { get; private set; }

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
        try
        {
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
        }
        catch (Exception e)
        {
            if (e is ApplicationException)
            {
                BenefitRefusalReason = e.Message;
            }
        }
        
        return Page();
    }
    
    
}