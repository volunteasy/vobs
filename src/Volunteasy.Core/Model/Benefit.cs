using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public record Benefit : IId, IOrganization
{
    public long Id { get; init; }

    public long OrganizationId { get; init; }
    
    public long AssistedId { get; init; }
    
    public Distribution? Distribution { get; set; }

    public long? DistributionId { get; init; }
    
    public long? Position { get; init; }

    public DateTime? ClaimedAt { get; set; }
    
    public RevokedBenefitReason? RevokedReason { get; set; }
    
    public IList<BenefitItem>? Items { get; init; }

    public bool IsClaimed => ClaimedAt != null;

    public bool RecentlyClaimed(DateTime now, int benefitEligibleTimeSpan)
        => // If the benefit was claimed up to `benefitEligibleTimeSpan` days ago,
            // beneficiary is not eligible to a new benefit
            IsClaimed && (now - ClaimedAt!).Value.Days < benefitEligibleTimeSpan;

    public bool IsInAnOpenDistribution(DateTime now)
        => !IsClaimed && (!Distribution?.Closed(now) ?? false);


}

public enum RevokedBenefitReason
{
    Revoked = 1,
    TooManyBenefitsForAddress = 2,
    TooManyBenefitsInPeriod = 3,
    StockCantAfford = 4,
    Canceled = 5,
}

public record DistributionBenefitAnalysisRequest
{
    public IEnumerable<BenefitDemandItem>? Items { get; init; }
}

public record BenefitProvision
{
    public long? DistributionId { get; init; }
    
    [Required]
    public BeneficiaryCreation? AssistedUser { get; init; }
    
    public IEnumerable<BenefitDemandItem>? Items { get; init; }
}

public record BenefitDetails
{
    public long Id { get; init; }
    
    public DateTime? ClaimedAt { get; init; }
    
    
    public long UserId { get; init; }
    
    public string? UserName { get; init; }
    
    
    public long? DistributionId { get; init; }
    
    public string? DistributionName { get; init; }
    
    public DateTime? DistributionDate { get; init; }
    
    
    public int ItemsCount { get; init; }
    
    public int? Position { get; set; }

    public IList<BenefitItemDetails>? Items { get; init; }
}


public record BenefitFilter{
    public long? AssistedId  { get; set; }
    public long? DistributionId  { get; set; }
    public DateTime? ClaimedUntil  { get; set; }
    public DateTime? ClaimedSince  { get; set; }
    public bool NotClaimedOnly  { get; set; } = false;

};