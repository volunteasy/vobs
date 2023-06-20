using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class User : IdBased
{
    public string? ExternalId { get; set; }
    
    [Required, MaxLength(11), MinLength(3)]
    public string? Document { get; set; }
    
    [Required, MaxLength(50), MinLength(1)]
    public string? Name { get; set; }
    
    [Required, EmailAddress]
    public string? Email { get; init; }
    
    public long? AddressId { get; init; }
    
    public Address? Address { get; init; }
    
    public string? PhoneAddress { get; set; }
}

public record UserRegistration
{
    [Required, MaxLength(11), MinLength(3)]
    public string? Document { get; init; }
    
    [Required, MaxLength(50), MinLength(1)]
    public string? Name { get; init; }
    
    [Required, EmailAddress]
    public string? Email { get; init; }
    
    [Required]
    public string? Password { get; init; }
    
    public Address? Address { get; init; }
    
    public string? PhoneAddress { get; set; }
}

public record UserCredentials
{
    [Required, EmailAddress]
    public string? Email { get; init; }
    
    [Required]
    public string? Password { get; init; }
}

public record UserDetails
{
    [Required, MaxLength(11), MinLength(3)]
    public string? Document { get; init; }
    
    [Required, MaxLength(50), MinLength(1)]
    public string? Name { get; init; }
    
    [Required, EmailAddress]
    public string? Email { get; init; }
}