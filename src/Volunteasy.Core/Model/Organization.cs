using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Organization : IdBased
{
    [Required, MaxLength(14), MinLength(14)]
    public string? Document { get; set; }
    
    [Required, MaxLength(255), MinLength(3)]
    public string? Name { get; set; }
    
    [Required, MaxLength(15)]
    public string? Slug { get; set; }

    public long AddressId { get; set; }
    
    [Required]
    public Address? Address { get; set; }
    
    [Required, MaxLength(32)]
    public string? PhoneNumber { get; set; }
    
    
    public IEnumerable<Membership>? Memberships { get; set; }
    
    public IEnumerable<Distribution>? Distributions { get; set; }
    
    public IEnumerable<Benefit>? Benefits { get; set; }

    public IEnumerable<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

    public IEnumerable<Resource>? Resources { get; set; }
}