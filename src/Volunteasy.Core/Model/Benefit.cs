using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Benefit : OrganizationBasedResource
{
    public long? DistributionId { get; set; }
    
    public Distribution? Distribution { get; set; }
    
    [Required]
    public long StockMovementId { get; set; }
    
    public StockMovement? StockMovement { get; set; }
    
    [Required]
    public long ResourceId { get; set; }
    
    public Resource? Resource { get; set; }
}