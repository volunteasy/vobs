using System.Linq.Expressions;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class MembershipService : IMembershipService
{
    private readonly Data _data;

    private readonly ISession _session;

    public MembershipService(Data data, ISession session)
    {
        _data = data;
        _session = session;
    }

    public async Task EnrollOrganization(long organizationId, long memberId, MembershipRole role)
    {
        try
        {
            var status = MembershipStatus.Pending;

            if (role == MembershipRole.Assisted || _session.IsOwner())
                status = MembershipStatus.Approved;
        
            await _data.Memberships.AddAsync(new Membership
            {
                MemberId = memberId,
                OrganizationId = organizationId,
                Role = role,
                Status = status,
                MemberSince = DateTime.Now.ToUniversalTime()
            });

            await _data.SaveChangesAsync();
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateMembershipException();
        }
    }

    public async Task LeaveOrganization(long orgId, long memberId)
    {
        if (!_session.IsUser(memberId) || !_session.IsOwner())
            throw new UserNotAuthorizedException();
        
        var membership = await GetMembershipById(orgId, memberId);
        
        if (membership == null) 
            throw new MembershipNotFoundException();
        
        _data.Memberships.Remove(membership);
        await _data.SaveChangesAsync();
    }

    public async Task ChangeMembershipRole(long orgId, long memberId, MembershipRole role)
    {
        // If new role is MembershipRole.Owner but agent is not owner, it fails
        // If membership does not belong to the logged in user or logged in user
        // is not organization owner, fails
        // TODO: Add test cases to check this authorization
        if ((role == MembershipRole.Owner && !_session.IsOwner()) || 
            (!_session.IsUser(memberId) && !_session.IsOwner()))
            throw new UserNotAuthorizedException();
        
        var membership = await GetMembershipById(orgId, memberId);
        if (membership == null)
            throw new MembershipNotFoundException();
        
        membership.Role = role;
        await _data.SaveChangesAsync();
    }
    
    public async Task ChangeMembershipStatus(long orgId, long memberId, MembershipStatus status)
    {
        if (status == MembershipStatus.Pending)
            return;
        
        if (!_session.IsUser(memberId) || !_session.IsOwner())
            throw new UserNotAuthorizedException();
        
        var membership = await GetMembershipById(orgId, memberId);
        if (membership == null)
            throw new MembershipNotFoundException();
        
        await _data.SaveChangesAsync();
    }

    public async Task<(IEnumerable<OrganizationMember>, string?)> ListMemberships(MembershipFilter filter, long pageToken)
    {
        if (filter.OrganizationId == null && filter.MemberId == null)
            throw new InvalidMembershipFilterException();

        var query = _data.Memberships.AsQueryable();

        query = new List<KeyValuePair<bool,  Expression<Func<Membership,bool>>>>
        {
            new(filter.OrganizationId != null, m => m.OrganizationId == filter.OrganizationId),
            new(filter.MemberId != null, m => m.MemberId == filter.MemberId),
            new(filter.Role != null, m => m.Role == filter.Role),
            new(filter.Status != null, m => m.Status == filter.Status),
            new(filter.MemberSince != null, m => m.MemberSince >= filter.MemberSince),
            new(filter.MemberUntil != null, m => m.MemberSince <= filter.MemberUntil),
        }.Where(queryFilter => queryFilter.Key)
            .Aggregate(query, (current, queryFilter) => current.Where(queryFilter.Value));

        return await query.Join(_data.Users,
                membership => membership.MemberId, user => user.Id, (membership, user) => new
                {
                    Membership = membership, User = user
                })
            .Join(_data.Organizations,
                mu => mu.Membership.OrganizationId, o => o.Id, (mu, organization) => new OrganizationMember
                {
                    Role = mu.Membership.Role,
                    Status = mu.Membership.Status,
                    MemberSince = mu.Membership.MemberSince,
                    OrganizationId = mu.Membership.OrganizationId,
                    MemberId = mu.Membership.MemberId,
                    MemberName = mu.User.Name,
                    OrganizationName = organization.Name
                }).Where(x => x.MemberId >= pageToken)
            .OrderBy(x => x.MemberId)
            .ThenBy(x => x.OrganizationId)
            .Paginate(x => filter.OrganizationId == null ? x.OrganizationId : x.MemberId);
    }

    private Task<Membership?> GetMembershipById(long orgId, long memberId)
        => _data.Memberships
            .SingleOrDefaultAsync(x
                => x.MemberId == memberId && x.OrganizationId == orgId);
}