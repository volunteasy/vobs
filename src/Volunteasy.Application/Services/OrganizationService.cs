using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly Data _data;

    private readonly ISession _session;

    public OrganizationService(Data data, ISession session)
    {
        _data = data;
        _session = session;
    }
    
    public async Task<Organization> CreateOrganization(Organization org)
    {
        var res = await _data.Organizations.AddAsync(org);
        await _data.Memberships.AddAsync(new Membership
        {
            MemberId = _session.UserId,
            OrganizationId = res.Entity.Id,
            Role = MembershipRole.Owner,
            Status = MembershipStatus.Approved,
            Active = true,
            MemberSince = DateTime.Now.ToUniversalTime()
        });

        await _data.SaveChangesAsync();
        return res.Entity;
    }

    public async Task<Organization> GetOrganizationById(long id)
    {
        var org = await _data.Organizations
            .SingleOrDefaultAsync(x => x.Id == id);
        
        return org switch
        {
            null => throw new OrganizationNotFoundException(), _ => org
        };
    }

    public async Task UpdateOrganizationById(long id, Organization organization)
    {
        var org = await _data.Organizations
            .SingleOrDefaultAsync(x => x.Id == id);

        if (org == null)
            throw new OrganizationNotFoundException();
        
        if (!IsUserOrganizationOwner(id, _session.UserId))
            throw new UserNotAuthorizedException();

        org.CoordinateX = organization.CoordinateX;
        org.CoordinateY = organization.CoordinateY;
        org.AddressName = organization.AddressName;
        org.Document = organization.Document;
        org.PhoneNumber = organization.PhoneNumber;
        org.Name = organization.Name;

        await _data.SaveChangesAsync();
    }

    public async Task CreateMembership(long organizationId, long memberId, MembershipRole role)
    {
        var status = MembershipStatus.Pending;

        if (role == MembershipRole.Assisted)
            status = MembershipStatus.Approved;
        
        var existingMembership = await _data.Memberships
            .SingleOrDefaultAsync(x => x.MemberId == memberId);

        if (existingMembership != null)
        {
            existingMembership.Active = true;
            existingMembership.Status = status;
            await _data.SaveChangesAsync();
            return;
        }
        
        await _data.Memberships.AddAsync(new Membership
        {
            MemberId = memberId,
            OrganizationId = organizationId,
            Role = role,
            Status = status,
            Active = true,
            MemberSince = DateTime.Now.ToUniversalTime()
        });

        await _data.SaveChangesAsync();
    }

    public async Task RemoveMembership(long orgId, long memberId)
    {
        var membership = await GetMembershipById(orgId, memberId);

        membership.Active = false;
        await _data.SaveChangesAsync();
    }

    public async Task ChangeMembershipType(long orgId, long memberId, MembershipRole role)
    {
        var membership = await GetMembershipById(orgId, memberId);
        membership.Role = role;
        
        await _data.SaveChangesAsync();
    }
    
    public async Task ChangeMembershipStatus(long orgId, long memberId, MembershipStatus status)
    {
        var membership = await GetMembershipById(orgId, memberId);
        membership.Status = status;
        
        await _data.SaveChangesAsync();
    }

    public async Task<(IEnumerable<OrganizationMember>, bool)> ListMemberships(MembershipFilter filter)
    {
        var query = _data.Memberships
            .Where(m => m.OrganizationId == filter.OrganizationId)
            .Where(m => m.Active);

        if (filter.Type != null)
            query = query.Where(x => x.Role == filter.Type);
        
        if (filter.Status != null)
            query = query.Where(x => x.Status == filter.Status);

        if (filter.MemberSince != null)
            query = query
                .Where(x => x.MemberSince >= filter.MemberSince);
        
        if (filter.MemberUntil != null)
            query = query
                .Where(x => x.MemberSince <= filter.MemberUntil);

        return await query.Join(
            _data.Users, 
            m => m.MemberId, 
            u => u.Id, 
            (membership, user) => new OrganizationMember
        {
            Role = membership.Role,
            Status = membership.Status,
            MemberSince = membership.MemberSince,
            OrganizationId = membership.OrganizationId,
            MemberId = membership.MemberId,
            MemberName = user.Name,
        }).Paginate(filter);
    }

    // This is not meant to be used as a CRUD method. It evaluates if the logged in user has access to it
    private async Task<Membership> GetMembershipById(long orgId, long memberId)
    {
        var membership = await _data.Memberships
            .SingleOrDefaultAsync(x 
                => x.MemberId == memberId && x.OrganizationId == orgId);

        if (membership == null ||
            (membership.MemberId != _session.UserId && // If membership does not belong to the logged in user
             !IsUserOrganizationOwner( // and they're not organization owners
                 membership.OrganizationId, _session.UserId)))
            throw new MembershipNotFoundException();

        return membership;
    }

    private bool IsUserOrganizationOwner(long orgId, long userId) =>
        _data.Memberships.Any(x =>
            x.OrganizationId == orgId &&
            x.Role == MembershipRole.Owner &&
            x.MemberId == userId);
}