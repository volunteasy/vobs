using System.ComponentModel.DataAnnotations;
using Volunteasy.Core.Enums;

namespace Volunteasy.Core.DTOs;

public record MembershipFilter : Filter
{
    public long? OrganizationId { get; init; }
    
    public long? MemberId { get; init; }
    
    public MembershipRole? Type { get; init; }
    
    public MembershipStatus? Status { get; init; }
    
    public DateTime? MemberSince { get; init; }
    
    public DateTime? MemberUntil { get; init; }
}

public record OrganizationMember
{
    public string? MemberName { get; init; }
    
    public string? OrganizationName { get; init; }
    
    public long OrganizationId { get; init; }
    
    public long MemberId { get; init; }
    
    public MembershipRole Role { get; init; }
    
    public MembershipStatus Status { get; init; }
    
    public DateTime MemberSince { get; init; }
}