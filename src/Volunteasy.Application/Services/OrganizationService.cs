using EntityFramework.Exceptions.Common;
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
        try
        {
            var res = await _data.Organizations.AddAsync(org);
            await _data.Memberships.AddAsync(new Membership
            {
                MemberId = _session.UserId,
                OrganizationId = res.Entity.Id,
                Role = MembershipRole.Owner,
                Status = MembershipStatus.Approved,
                MemberSince = DateTime.Now.ToUniversalTime()
            });

            await _data.SaveChangesAsync();
            return res.Entity;
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateOrganizationException();
        }
        
    }

    public async Task<(IEnumerable<Organization>, bool)> ListOrganizations(OrganizationFilter filter)
    {
        var query = _data.Organizations.AsQueryable();

        if (filter.Name != null)
            query = query.Where(x => 
                x.Name != null && x.Name.Contains(filter.Name));

        return await query.Paginate(filter);
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

    public async Task RemoveMembership(long orgId, long memberId)
    {
        var membership = await GetMembershipById(orgId, memberId);
        // TODO: Send revoked membership e-mail if admin is removing;

        _data.Memberships.Remove(membership);
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
        if (status == MembershipStatus.Declined)
        {
            await RemoveMembership(orgId, memberId);
            return;
        }

        var membership = await GetMembershipById(orgId, memberId);
        membership.Status = status;
        
        await _data.SaveChangesAsync();
    }

    public async Task<(IEnumerable<OrganizationMember>, bool)> ListMemberships(MembershipFilter filter)
    {
        if (filter.OrganizationId == null && filter.MemberId == null)
            throw new InvalidMembershipFilterException();


        var query = _data.Memberships.AsQueryable();

        if (filter.OrganizationId != null)
            query = query.Where(x => x.OrganizationId == filter.OrganizationId);
        
        if (filter.MemberId != null)
            query = query.Where(x => x.MemberId == filter.MemberId);

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