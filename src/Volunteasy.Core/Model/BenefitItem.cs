using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public record BenefitItem : IOrganization
{
    public long BenefitId { get; init; }
    
    public long OrganizationId { get; init; }
    
    public long ResourceId { get; init; }
    
    public long StockMovementId { get; init; }
    
    [Required, Range(1, double.MaxValue)]
    public decimal Quantity { get; set; }
    
    public StockMovement? StockMovement { get; init; }
    
    public Resource? Resource { get; init; }
}


public record BenefitItemDetails
{
    public long ResourceId { get; init; }
    
    public string? ResourceName { get; init; }
    
    public long StockMovementId { get; init; }
    
    [Required, Range(1, double.MaxValue)]
    public decimal Quantity { get; init; }
}


public record BenefitDemandItem
{
    
    [Required, Range(1, double.MaxValue)]
    public decimal Quantity { get; init; }
}
