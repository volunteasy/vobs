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
    
    
}

public enum RevokedBenefitReason
{
    Revoked = 1,
    TooManyBenefitsForAddress = 2,
    TooManyBenefitsInPeriod = 3,
    StockCantAfford = 4,
    Canceled = 5,
}

public record BenefitAnalysisRequest
{
    [Required]
    public long DistributionId { get; init; }
    public IEnumerable<BenefitDemandItem>? Items { get; init; }
}

public record BenefitProvision
{
    public long? DistributionId { get; init; }
    
    [Required]
    public AssistedUser? AssistedUser { get; init; }
    
    public IEnumerable<BenefitDemandItem>? Items { get; init; }
}

public record AssistedUser
{
    [Required, MaxLength(11), MinLength(3)]
    public string? Document { get; init; }
    
    [MaxLength(50), MinLength(1)]
    public string? Name { get; init; }
    public Address? Address { get; init; }
    
    public string? Phone { get; set; }
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


public record BenefitAnalysisResult
{
    
}


public record BenefitFilter{
    public long? AssistedId  { get; set; }
    public long? DistributionId  { get; set; }
    public DateTime? ClaimedUntil  { get; set; }
    public DateTime? ClaimedSince  { get; set; }
    public bool NotClaimedOnly  { get; set; } = false;

};