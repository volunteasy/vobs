using System.ComponentModel.DataAnnotations;
using Volunteasy.Core.VOs;

namespace Volunteasy.Core.Model;

public class Organization : IdBased
{
    [Required, MaxLength(14), MinLength(14)]
    public string? Document { get; set; }
    
    [Required, MaxLength(255), MinLength(3)]
    public string? Name { get; set; }
    
    [Required]
    public Address? Address { get; set; }
    
    [Required, MaxLength(32)]
    public string? PhoneNumber { get; set; }

    public ICollection<Membership>? Memberships { get; set; }
}