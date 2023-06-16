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
}