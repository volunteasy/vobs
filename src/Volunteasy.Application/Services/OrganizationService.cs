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
    
    public async Task<Organization> CreateOrganization(OrganizationRegistration org)
    {
        try
        {
            var res = await _data.Organizations.AddAsync(new Organization
            {
                Document = org.Document,
                Name = org.Name,
                PhoneNumber = org.PhoneNumber,
                Address = org.Address
            });
            
            await _data.Memberships.AddAsync(new Membership
            {
                MemberId = _session.UserId,
                Role = MembershipRole.Owner,
                Status = MembershipStatus.Approved,
                MemberSince = DateTime.Now.ToUniversalTime()
            });

            await _data.Resources.AddAsync(new Resource
            {
                Name = "Cesta básica"
            });
            
            await _data.SaveChangesAsync();
            return res.Entity;
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateOrganizationException();
        }
        
    }

    public async Task<(IEnumerable<Organization>, string?)> ListOrganizations(OrganizationFilter filter, long pageToken)
    {
        var query = _data.Organizations.AsQueryable();

        if (filter.Name != null)
            query = query.Where(x => 
                x.Name != null && x.Name.Contains(filter.Name));

        return await query
            .Where(x => x.Id >= pageToken)
            .OrderBy(x => x.Id)
            .Paginate(x => x.Id);
    }

    public async Task<Organization> GetOrganizationById(long id)
    {
        var org = await _data.Organizations
            .Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Id == id);
        
        return org switch
        {
            null => throw new OrganizationNotFoundException(), _ => org
        };
    }

    public async Task UpdateOrganizationById(long id, OrganizationRegistration organization)
    {
        var org = await _data.Organizations
            .SingleOrDefaultAsync(x => x.Id == id);

        if (org == null)
            throw new OrganizationNotFoundException();
        
        if (!IsUserOrganizationOwner(id, _session.UserId))
            throw new UserNotAuthorizedException();

        org.Address = organization.Address;
        org.Document = organization.Document;
        org.PhoneNumber = organization.PhoneNumber;
        org.Name = organization.Name;

        await _data.SaveChangesAsync();
    }

    private bool IsUserOrganizationOwner(long orgId, long userId) =>
        _data.Memberships.Any(x =>
            x.OrganizationId == orgId &&
            x.Role == MembershipRole.Owner &&
            x.MemberId == userId);
}