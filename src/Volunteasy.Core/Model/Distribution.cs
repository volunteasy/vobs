using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

[DisplayName("Distribuição")]
public class Distribution : IId, IOrganization
{
    public long Id { get; set; }

    public long OrganizationId { get; set; }
    
    [MinLength(1), MaxLength(100)]
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime StartsAt { get; set; }
    
    public DateTime EndsAt { get; set; }
    
    public int MaxBenefits { get; set; }
    
    public bool Canceled { get; set; }
    
    public IEnumerable<Benefit>? Benefits { get; set; }

    public bool CanAcceptNewBenefits(DateTime currentDate, int currentBenefitsNumber)
        => 
            // Checks if the distribution has not been canceled
            !Canceled && 
               
            // Checks if current date is before the end of the distribution
            DateTime.Compare(EndsAt, currentDate) >= 0 &&
               
            // Validates if the maximum number of benefits has not been yet reached
            currentBenefitsNumber < MaxBenefits;
}

public struct DistributionCreationProperties
{
    [Required, MinLength(1), MaxLength(100)]
    public string? Name { get; init; }
    
    [Required]
    public string? Description { get; init; }
    
    [Required]
    public DateTime StartsAt { get; init; }
    
    [Required]
    public DateTime EndsAt { get; init; }
    
    [Required, Range(0, int.MaxValue)]
    public int MaxBenefits { get; init; }

    public Distribution ToDistribution()
        => new Distribution
        {
            Description = Description,
            StartsAt = StartsAt,
            EndsAt = EndsAt,
            MaxBenefits = MaxBenefits,
            Name = Name
        };
}

public record DistributionFilter
{
    public DateTime? OccursAt { get; set; }
    
    [MinLength(1), MaxLength(100)]
    public string? Name { get; set; }
}