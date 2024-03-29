using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;

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
            !Closed(currentDate) &&
               
            // Validates if the maximum number of benefits has not been yet reached
            currentBenefitsNumber < MaxBenefits;

    public bool Closed(DateTime currentDate)
        =>
            // Checks if the distribution is canceled
            Canceled ||

            // Checks if current date is before the end of the distribution
            currentDate > EndsAt;
    
    
    public void ValidateNewBeneficiary(long beneficiaryId)
    {
        // Checks if beneficiary is already enrolled in this distribution
        if (Benefits?.Any(b => b.AssistedId == beneficiaryId) ?? false)
            throw new BenefitUnauthorizedForUserException();

        // Checks if distribution is able to accept new beneficiaries enrollments
        if (!CanAcceptNewBenefits(DateTime.UtcNow, Benefits?.Count() ?? 0))
            throw new DistributionClosedOrFullException();
    }
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


public record DistributionDto : IId, IOrganization
{
    public long Id { get; set; }

    public long OrganizationId { get; set; }
    
    [MinLength(1), MaxLength(100)]
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime StartsAt { get; set; }
    
    public DateTime EndsAt { get; set; }
    
    public int MaxBenefits { get; set; }
    
    public int RemainingBenefits { get; init; }
    
    public bool Canceled { get; set; }

    public DistributionStats Stats { get; init; } = new();
    
    public BenefitStats? Benefit { get; init; }
}

public record DistributionStats
{
    public int ClaimedBenefits { get; init; }
    
    public int BenefitsToClaim { get; set; }
}

public record BenefitStats
{
    public long BenefitId { get; init; }
    
    public long Position { get; init; }

    public DateTime? ClaimedAt { get; init; }
    
    public RevokedBenefitReason? RevokedReason { get; init; }
}