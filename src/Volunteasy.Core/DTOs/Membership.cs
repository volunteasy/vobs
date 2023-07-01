using System.Text.Json.Serialization;
using Volunteasy.Core.Enums;

namespace Volunteasy.Core.DTOs;

public record MembershipFilter
{
    public long? OrganizationId { get; set; }
    public long? MemberId { get; set; }
    public MembershipRole? Role { get; set; }
    public MembershipStatus? Status { get; set; }
    public DateTime? MemberSince { get; set; }
    public DateTime? MemberUntil { get; set; }
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
}