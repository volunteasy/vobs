using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class StockMovement : OrganizationBasedResource
{
    [Required]
    public string? Description { get; set; }
    
    [Required]
    public StockMovementType Type { get; set; }
    
    [Required]
    public decimal Quantity { get; set; }
    
    [Required]
    public long ResourceId { get; set; }
    
    public Resource? Resource { get; set; }
    
    public long? ImportId { get; set; }
}

public enum StockMovementType
{
    Input = 1,
    Output = 2
}