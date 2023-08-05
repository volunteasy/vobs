using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.App.Pages.Shared;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Pages.Quero;

public class IndexModel : OrganizationPageModel
{

    private readonly IDistributionService _distributions;

    private readonly IBenefitService _benefitService;

    private readonly IVolunteasyContext _ctx;

    public PaginatedList<DistributionDto> Distributions { get; private set; } =
        new(new List<DistributionDto>());

    public BenefitDetails? NextBenefitToClaim { get; private set; }

    public PaginatedList<BenefitDetails> PreviousBenefits { get; private set; } = 
        new(new List<BenefitDetails>());

    public IndexModel(IOrganizationService organizations, ILogger<IndexModel> logger,
        IDistributionService distributions, IBenefitService benefitService, IVolunteasyContext ctx) : base(organizations)
    {
        _distributions = distributions;
        _benefitService = benefitService;
        _ctx = ctx;
    }
    
    [FromQuery]
    public long? PageToken { get; set; }

    public override async Task<ActionResult> OnGet()
    {
        if (!HttpContext.User.Identity?.IsAuthenticated ?? true)
            return Challenge(CookieAuthenticationDefaults.AuthenticationScheme);

        Distributions = await _distributions.ListDistributions(new DistributionFilter(), PageToken ?? 0);

        try
        {
            NextBenefitToClaim = await _benefitService.GetNextBenefit(_ctx.UserId);
        }
        catch (BenefitNotFoundException) {}
        
        try
        {
            var (previousBenefits, next) = await _benefitService.ListBenefits(new BenefitFilter
            {
                ClaimedUntil = DateTime.UtcNow
            }, 0);

            PreviousBenefits = new PaginatedList<BenefitDetails>(previousBenefits);
        }
        catch (BenefitNotFoundException) {}
        
        

        return await base.OnGet();
    }
    
    
}