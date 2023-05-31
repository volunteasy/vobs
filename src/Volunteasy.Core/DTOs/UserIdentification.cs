using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.DTOs;

public record UserIdentification
{
    [Required, MaxLength(11), MinLength(3)]
    public string? Document { get; init; }
    
    [Required, MaxLength(50), MinLength(3)]
    public string? Name { get; init; }
    
    [Required, EmailAddress]
    public string? Email { get; init; }
    
    [Required]
    public string? Password { get; init; }
}

public record UserCredentials
{
    [Required, EmailAddress]
    public string? Email { get; init; }
    
    [Required]
    public string? Password { get; init; }
}

public record UserToken
{
    public string? Token { get; init; }
}