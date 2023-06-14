using Volunteasy.Core.Enums;

namespace Volunteasy.Core.DTOs;

public record OrganizationFilter : Filter
{
    public string? Name { get; init; }
}