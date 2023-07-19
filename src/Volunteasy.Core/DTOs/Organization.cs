using System.ComponentModel.DataAnnotations;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.DTOs;

public record OrganizationFilter 
{
    public string? Name { get; init; }
}

public record OrganizationRegistration(
    [Required, MaxLength(14), MinLength(14)]
    string? Document,
    [Required, MaxLength(255), MinLength(3)]
    string? Name,
    [Required]
    Address? Address,
    [Required, MaxLength(32)]
    string? PhoneNumber,
    [Required, MaxLength(15)]
    string? Slug
 );


public record OrganizationDetails
{
    [Required] public long Id { get; init; }
    
    [Required, MaxLength(255), MinLength(3)]
    public string Name { get; init; } = null!;
    
    [Required, MaxLength(32), Phone]
    public string PhoneNumber { get; init; } = null!;

    [Required] public Address Address { get; init; } = new();

    [Required] public OrganizationStats Stats { get; init; } = new();

    [Required] public MembershipStats? Membership { get; init; }
}

public record OrganizationStats
{
    public int AssistedPeopleCount { get; init; }
    
    public int NextDistributionsCount { get; init; }
}

public record MembershipStats
{
    public MembershipRole Role { get; init; }

    public MembershipStatus Status { get; init; }
    
    public DateTime MemberSince { get; init; }
    
    public int BenefitsReceived { get; init; }
}