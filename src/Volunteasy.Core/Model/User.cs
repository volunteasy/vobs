using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class User : Resource
{
    public string? ExternalId { get; set; }
    
    [Required, MaxLength(11), MinLength(3)]
    public string? Document { get; set; }
    
    [Required, MaxLength(50), MinLength(3)]
    public string? Name { get; set; }
}