using System.ComponentModel.DataAnnotations;

namespace Volunteasy.Core.Model;

public class Address : IdBased
{
    public string? AddressName { get; set; }
    
    [Required]
    public string? AddressNumber { get; set; }
    
    [Required]
    public string? ZipCode { get; set; }
    
    public float CoordinateX { get; set; }
    
    public float CoordinateY { get; set; } 0
}