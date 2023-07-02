using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class StockMovement : OrganizationBasedResource
{
    public string? Description { get; set; }
    
    [Required]
    public StockMovementType Type { get; set; }
    
    [Required]
    public decimal Quantity { get; set; }
    
    public DateTime Date { get; set; }
    
    [Required]
    public long ResourceId { get; set; }
    
    public Resource? Resource { get; set; }
    
    public long? ImportId { get; set; }
}

public enum StockMovementType
{
    Input = 1,
    Output = 2,
    Reserved = 3,
}

public record StockMovementEdition(
    [Required] string? Description, 
    [Required] StockMovementType Type, 
    [Required, Range(double.MinValue, double.MaxValue)] decimal Quantity, 
    long ResourceId);

public record StockMovementFilter(
    [Required] long ResourceId, StockMovementType? Type, DateTime? Since, DateTime? Until);
  
