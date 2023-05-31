using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class User : Resource
{
    public string? ExternalId { get; init; }
    
    [Required, MaxLength(11), MinLength(3)]
    public string? Document { get; init; }
    
    [Required, MaxLength(50), MinLength(3)]
    public string? Name { get; init; }
}