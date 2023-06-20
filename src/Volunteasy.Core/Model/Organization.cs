using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Organization : IdBased
{
    [Required, MaxLength(14), MinLength(14)]
    public string? Document { get; set; }
    
    [Required, MaxLength(255), MinLength(3)]
    public string? Name { get; set; }

    public long AddressId { get; set; }
    
    [Required]
    public Address? Address { get; set; }
    
    [Required, MaxLength(32)]
    public string? PhoneNumber { get; set; }
}