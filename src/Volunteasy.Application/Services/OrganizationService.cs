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

    private readonly IVolunteasyContext _session;

    public OrganizationService(Data data, IVolunteasyContext session)
    {
        _data = data;
        _session = session;
    }
    
    public async Task<Organization> CreateOrganization(OrganizationRegistration org)
    {
        try
        {
            var distDate = DateTime.Now.AddDays(1);
            var distDesc = @$"Distribuições são eventos pré-definidos pela organização onde ocorre um mutirão de doações para um número definido de pessoas. 
Neste caso, esta é uma distribuição para 100 pessoas que ocorre no dia {distDate:dd/MM/yyyy} às {distDate:HH}h e termina às {distDate.AddHours(5):HH}h. 
Conforme os asisstidos se inscrevem para receber na distribuição, uma fila é gerada para que você possa controlar as entregas de forma justa para todos, mas você ainda pode entregar um benefício normalmente para quem não se inscreveu através da página inicial da organização.";
            
            var res = await _data.Organizations.AddAsync(new Organization
            {
                Document = org.Document,
                Name = org.Name,
                PhoneNumber = org.PhoneNumber,
                Address = org.Address,
                Slug = org.Slug,
                Memberships = new List<Membership>
                {
                    new()
                    {
                        MemberId = _session.UserId,
                        Role = MembershipRole.Owner,
                        Status = MembershipStatus.Approved,
                        MemberSince = DateTime.Now.ToUniversalTime()
                    }
                },
                Resources = new List<Resource>
                {
                    new()
                    {
                        Name = "Cesta básica"
                    }
                },
                Distributions = new List<Distribution>
                {
                    new()
                    {
                        Name = "Exemplo de distribuição de cestas-básicas",
                        StartsAt = distDate.ToUniversalTime(),
                        EndsAt = distDate.AddHours(5).ToUniversalTime(),
                        MaxBenefits = 100,
                        Description = distDesc
                    }
                }
            });
            
            await _data.SaveChangesAsync();
            return res.Entity;
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateOrganizationException();
        }
        
    }

    public async Task<PaginatedList<OrganizationDetails>> ListOrganizations(OrganizationFilter filter, long pageToken)
    {
        var query = _data.OrganizationDetails(_session.UserId).AsQueryable();

        if (filter.Name != null)
            query = query.Where(x => x.Name.Contains(filter.Name));

        return await query
            .Where(x => x.Id >= pageToken)
            .OrderBy(x => x.Id)
            .PaginateList(x => x.Id);
    }

    public async Task<OrganizationDetails> GetOrganizationById(long id)
    {
        var org = await _data
            .OrganizationDetails(_session.UserId)
            .SingleOrDefaultAsync(x => x.Id == id);
        
        return org switch
        {
            null => throw new OrganizationNotFoundException(), _ => org
        };
    }

    public async Task<Organization> GetOrganizationBySlug(string slug)
    {
        var org = await _data.Organizations
            .Include(o => o.Address)
            .SingleOrDefaultAsync(x => x.Slug == slug);
        
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

        org.Address = organization.Address;
        org.Document = organization.Document;
        org.PhoneNumber = organization.PhoneNumber;
        org.Name = organization.Name;

        await _data.SaveChangesAsync();
    }
}