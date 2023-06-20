using Volunteasy.Core.Enums;

namespace Volunteasy.Core.DTOs;

public record MembershipFilter : PaginationDetails
{
    public long? OrganizationId;
    public long? MemberId;
    public MembershipRole? Role;
    public MembershipStatus? Status;
    public DateTime? MemberSince;
    public DateTime? MemberUntil;
}

public record OrganizationMember
{
    public string? MemberName;
    public string? OrganizationName;
    public long OrganizationId;
    public long MemberId;
    public MembershipRole Role;
    public MembershipStatus Status;
    public DateTime MemberSince;
}