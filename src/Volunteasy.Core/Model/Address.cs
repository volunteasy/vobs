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
    
    public float CoordinateY { get; set; }
    
    public override string ToString()
    {
        var formattedAddress = string.Empty;

        // Add AddressName, if available
        if (!string.IsNullOrEmpty(AddressName))
        {
            formattedAddress += AddressName + ", ";
        }

        // Add AddressNumber (Required field)
        if (!string.IsNullOrEmpty(AddressNumber))
        {
            formattedAddress += "Nº " + AddressNumber + ", ";
        }

        // Add ZipCode (Required field)
        if (!string.IsNullOrEmpty(ZipCode))
        {
            formattedAddress += "CEP: " + ZipCode + ", ";
        }

        // Add more location details if necessary (e.g., neighborhood, city, state, country)

        // Trim trailing comma and space and return the formatted address
        return formattedAddress.TrimEnd(',', ' ');
    }
    
    public string ToStringWithoutCpf()
    {
        var formattedAddress = string.Empty;

        // Add AddressName, if available
        if (!string.IsNullOrEmpty(AddressName))
        {
            formattedAddress += AddressName + ", ";
        }

        // Add AddressNumber (Required field)
        if (!string.IsNullOrEmpty(AddressNumber))
        {
            formattedAddress += "Nº " + AddressNumber + ", ";
        }

        // Trim trailing comma and space and return the formatted address
        return formattedAddress.TrimEnd(',', ' ');
    }
}