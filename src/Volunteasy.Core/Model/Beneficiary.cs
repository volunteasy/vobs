using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Beneficiary : IId, IOrganization
{
    public long Id { get; set; }
    
    public long OrganizationId { get; set; }

    [Required, MaxLength(50), MinLength(1)]
    public string Name { get; set; } = "";

    [Required, MaxLength(11), MinLength(3)]
    public string Document { get; set; } = "";

    [Phone] 
    public string Phone { get; set; } = "";

    [EmailAddress] 
    public string Email { get; init; } = "";
    
    [Required]
    public DateTime BirthDate { get; set; }
    
    public long AddressId { get; set; }
    
    public Address? Address { get; init; }
    
    public IEnumerable<Benefit> Benefits = null!;
}

public record AssistedCreation
{
    [Required, MaxLength(50), MinLength(1)]
    public string Name { get; set; } = "";

    [Required, MaxLength(11), MinLength(3)]
    public string Document { get; set; } = "";

    [Phone] 
    public string Phone { get; set; } = "";

    [EmailAddress] 
    public string Email { get; init; } = "";

    [Required]
    public Address Address { get; init; } = null!;
}