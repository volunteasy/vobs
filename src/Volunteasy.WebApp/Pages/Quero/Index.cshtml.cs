using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.WebApp.Pages.Quero;

public class IndexModel : OrganizationPageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly IDistributionService _distributions;

    private readonly IBenefitService _benefitService;

    private readonly IBenefitProvisionService _provision;

    private readonly IVolunteasyContext _ctx;

    public PaginatedList<DistributionDto> Distributions { get; private set; } =
        new(new List<DistributionDto>());

    public BenefitDetails? NextBenefitToClaim { get; private set; }

    public IndexModel(IOrganizationService organizations, ILogger<IndexModel> logger,
        IDistributionService distributions, IBenefitService benefitService, IVolunteasyContext ctx, IBenefitProvisionService provision) : base(organizations)
    {
        _logger = logger;
        _distributions = distributions;
        _benefitService = benefitService;
        _ctx = ctx;
        _provision = provision;
    }
    
    [FromQuery]
    public long? PageToken { get; set; }

    public override async Task<ActionResult> OnGet()
    {
        if (!HttpContext.User.Identity?.IsAuthenticated ?? true)
            return Challenge();

        Distributions = await _distributions.ListDistributions(new DistributionFilter(), PageToken ?? 0);

        try
        {
            NextBenefitToClaim = await _benefitService.GetNextBenefit(_ctx.UserId);
        }
        catch (BenefitNotFoundException) {}

        return await base.OnGet();
    }
    
    
}