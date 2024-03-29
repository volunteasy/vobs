using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;

namespace Volunteasy.Core.Model;

[DisplayName("Beneficiário")]
public class Beneficiary : IId, IOrganization
{
    public long Id { get; set; }

    public long OrganizationId { get; set; }

    [Required, MaxLength(50), MinLength(1)]
    public string Name { get; set; } = "";

    [Required, MaxLength(11), MinLength(3)]
    public string Document { get; set; } = "";

    [Phone] public string? Phone { get; set; } = "";

    [EmailAddress] public string? Email { get; set; } = "";

    [Required] public DateTime BirthDate { get; set; }

    [Required]
    public DateTime MemberSince { get; set; }

    public bool Active { get; set; }
    
    public long? AddressId { get; set; }
    
    public Address? Address { get; set; }
    
    public IEnumerable<Benefit> Benefits = null!;
}

public record BeneficiaryCreation
{
    [Required, MaxLength(50), MinLength(1)]
    public string Name { get; init; } = "";

    [Required, MaxLength(11), MinLength(3)]
    public string Document { get; init; } = "";
    
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Phone { get; init; } = "";
    
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Email { get; init; } = "";
    
    [Required] public DateTime? BirthDate { get; init; }
    
    public Address? Address { get; set; }

    public Beneficiary ToBeneficiary()
    {
        return new Beneficiary
        {
            Name = Name,
            Document = Document,
            Phone = Phone,
            Email = Email,
            BirthDate = BirthDate?.Date.ToUniversalTime().Date ?? new DateTime(),
            Address = Address
        };
    }
}

public record BeneficiaryResume
{
    public long Id { get; init; }
    
    public long OrganizationId { get; init; }
    
    [MaxLength(50), MinLength(1)]
    public string Name { get; init; } = "";

    [MaxLength(11), MinLength(3)]
    public string Document { get; init; } = "";

    [Phone] 
    public string Phone { get; init; } = "";

    [EmailAddress] 
    public string Email { get; init; } = "";
    
    [Required] public DateTime BirthDate { get; set; }

    [Required]
    public DateTime MemberSince { get; set; }

    public Address? Address { get; init; }
    
    public bool Active { get; init; }
}

public record BeneficiaryFilter
{
    [Required]
    public long OrganizationId { get; init; }
    
    public string? Name { get; init; }
    
    public string? Document { get; init; }
    
    public string? Phone { get; init; }
}

public class BeneficiaryKey
{
    public string Document { get; set; } = "";
    
    public DateTime? BirthDate { get; set; }
}