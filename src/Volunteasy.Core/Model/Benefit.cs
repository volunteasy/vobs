using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public record Benefit
{
    public long Id { get; init; }

    public long OrganizationId { get; init; }
    
    public long AssistedId { get; init; }

    public long? DistributionId { get; init; }

    public DateTime? ClaimedAt { get; init; }
    
    public RevokedBenefitReason? RevokedReason { get; init; }
    
    public IEnumerable<BenefitItem>? Items { get; init; }
}

public enum RevokedBenefitReason
{
    Revoked = 1,
    TooManyBenefitsForAddress = 2,
    TooManyBenefitsInPeriod = 3,
    StockCantAfford = 4,
    Canceled = 5,
}

public record BenefitItem
{
    public long BenefitId { get; init; }
    
    public long ResourceId { get; init; }
    
    public long StockMovementId { get; init; }
    
    [Required, Range(1, double.MaxValue)]
    public decimal Quantity { get; init; }
    
    public StockMovement? StockMovement { get; init; }
}

public record BenefitDemand
{
    public long? UserId { get; init; }
    public long? DistributionId { get; init; }
    
    public UserRegistration? NewShallowUser { get; init; }
    
    public IEnumerable<BenefitDemandItem>? Items { get; init; }
}

public record BenefitDemandItem
{
    [Required]
    public long ResourceId { get; init; }
    
    [Required, Range(1, double.MaxValue)]
    public decimal Quantity { get; init; }
}

public record BenefitDemandAssisted
{
    public string? Document { get; init; }
    
    public Address? Address { get; init; }
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

    public IEnumerable<BenefitItem>? Items { get; init; }
}

public record BenefitItemDetails
{
    
    public long ResourceId { get; init; }
    
    public string? ResourceName { get; init; }
    
    
    public long StockMovementId { get; init; }
    
    [Required, Range(1, double.MaxValue)]
    public decimal Quantity { get; init; }
    
    public StockMovement? StockMovement { get; init; }
}

public record BenefitFilter(long? DistributionId, DateTime? ClaimedBefore, DateTime? ClaimedAfter);