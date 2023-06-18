using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Resource : IdBased
{
    [Required]
    public string? Name { get; set; }
}