using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Resource
{
    public long Id { get; set; }

    [Required] public long OrganizationId { get; set; }
        
    [Required, MaxLength(255)] public string? Name { get; set; }
    
}

