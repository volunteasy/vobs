using System.ComponentModel.DataAnnotations;
using Volunteasy.Core.VOs;

namespace Volunteasy.Core.Model;

public class Organization : Resource
{
    [Required, MaxLength(14), MinLength(14)]
    public string? Document { get; set; }
    
    [Required, MaxLength(255), MinLength(3)]
    public string? Name { get; set; }
    
    public float CoordinateX { get; set; }
    
    public float CoordinateY { get; set; }
    
    public string? AddressName { get; set; }
    
    [Required, MaxLength(32)]
    public string? PhoneNumber { get; set; }

    public ICollection<Membership>? Memberships { get; set; }
}