using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IOrganizationService
{
    public Task<Organization> CreateOrganization(Organization registration);
    public Task<(IEnumerable<Organization>, bool)> ListOrganizations(OrganizationFilter filter);
    public Task<Organization> GetOrganizationById(long id);
    public Task UpdateOrganizationById(long id, Organization identification);
    public Task CreateMembership(long organizationId, long memberId, MembershipRole role);
    public Task RemoveMembership(long orgId, long memberId);
    public Task ChangeMembershipType(long orgId, long memberId, MembershipRole role);
    public Task ChangeMembershipStatus(long orgId, long memberId, MembershipStatus status);
    public Task<(IEnumerable<OrganizationMember>, bool)> ListMemberships(MembershipFilter filter);
}