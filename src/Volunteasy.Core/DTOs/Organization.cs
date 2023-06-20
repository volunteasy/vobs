using System.ComponentModel.DataAnnotations;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.DTOs;

public record OrganizationFilter 
{
    public string? Name { get; init; }

    public PaginationDetails Pagination { get; init; } = new();
}

public record OrganizationRegistration(
    [Required, MaxLength(14), MinLength(14)]
    string? Document,
    [Required, MaxLength(255), MinLength(3)]
    string? Name,
    [Required]
    Address? Address,
    [Required, MaxLength(32)]
    string? PhoneNumber
 );