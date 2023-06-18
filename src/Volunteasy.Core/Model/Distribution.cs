using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Distribution : OrganizationBasedResource
{
    [Required, MinLength(1), MaxLength(100)]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
    
    [Required]
    public DateTime OccursAt { get; set; }
    
    [Required, Range(0, int.MaxValue)]
    public int MaxBenefits { get; set; }
}