using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IMembershipService
{
    public Task EnrollOrganization(long organizationId, long memberId, MembershipRole role);
    
    public Task LeaveOrganization(long orgId, long memberId);
    
    public Task ChangeMembershipRole(long orgId, long memberId, MembershipRole role);
    
    public Task ChangeMembershipStatus(long orgId, long memberId, MembershipStatus status);
    
    public Task<(IEnumerable<OrganizationMember>, string?)> ListMemberships(MembershipFilter filter);
}