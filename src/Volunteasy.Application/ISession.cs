using Volunteasy.Core.Enums;

namespace Volunteasy.Application;

public interface ISession
{
    long UserId { get; }

    MembershipRole GetMembershipRole(long organizationId);
    
    bool IsUser(long userId) => userId == UserId;
    
    bool CanAccessOrgAs(long organizationId, params MembershipRole[] wantedRole)
        => wantedRole.Length != 0 && wantedRole.Contains(
            GetMembershipRole(organizationId));
    
    bool IsSelfOrOrganizationOwner(long organizationId, long memberId)
        => IsUser(memberId) || CanAccessOrgAs(organizationId, MembershipRole.Owner);
}