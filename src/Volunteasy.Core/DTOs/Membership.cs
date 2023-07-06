using System.Text.Json.Serialization;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.DTOs;

public record MembershipFilter
{
    public long? OrganizationId { get; init; }
    
    public string? OrganizationName { get; init; }
    
    public string? MemberName { get; init; }
    public long? MemberId { get; init; }
    public MembershipRole? Role { get; init; }
    public MembershipStatus? Status { get; init; }
    public DateTime? MemberSince { get; init; }
    public DateTime? MemberUntil { get; init; }
}

public record OrganizationMember
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MemberName { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OrganizationName { get; set; }
    public long OrganizationId { get; set; }
    public long MemberId { get; set; }
    public MembershipRole Role { get; set; }
    public MembershipStatus Status { get; set; }
    public DateTime MemberSince { get; set; }
    
    public int NextDistributionsNumber { get; set; }
    
    public int DistributionsNumber { get; set; }
    
    public int MembershipsNumber { get; set; }
    
}