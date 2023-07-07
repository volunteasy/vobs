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

    public async Task<PaginatedList<OrganizationResume>> ListOrganizations(OrganizationFilter filter, long pageToken)
    {
        var query = _data.Organizations.AsQueryable();

        if (filter.Name != null)
            query = query.Where(x => 
                x.Name != null && x.Name.Contains(filter.Name));

        return await query
            .Where(x => x.Id >= pageToken)
            .Select(o => new OrganizationResume
            {
                Id = o.Id,
                Address = o.Address,
                Document = o.Document,
                Name = o.Name,
                PhoneNumber = o.PhoneNumber,
                NextDistributionsNumber = _data.Distributions.Count(x => x.OrganizationId == o.Id && x.StartsAt >= DateTime.Now.ToUniversalTime()),
                DistributionsNumber = _data.Distributions.Count(x => x.OrganizationId == o.Id && !x.Canceled),
                MembershipsNumber = _data.Memberships.Count(x => x.OrganizationId == o.Id && x.Role == MembershipRole.Assisted)
            })
            .OrderBy(x => x.Id)
            .PaginateList(x => x.Id);
    }

    public async Task<OrganizationResume> GetOrganizationById(long id)
    {
        var org = await _data.Organizations
            .Include(x => x.Address)
            .Select(o => new OrganizationResume
            {
                Id = o.Id,
                Address = o.Address,
                Document = o.Document,
                Name = o.Name,
                PhoneNumber = o.PhoneNumber,
                NextDistributionsNumber = _data.Distributions.Count(x => x.OrganizationId == o.Id && x.StartsAt >= DateTime.Now.ToUniversalTime()),
                DistributionsNumber = _data.Distributions.Count(x => x.OrganizationId == o.Id && !x.Canceled),
                MembershipsNumber = _data.Memberships.Count(x => x.OrganizationId == o.Id && x.Role == MembershipRole.Assisted)
            })
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

        org.Address = organization.Address;
        org.Document = organization.Document;
        org.PhoneNumber = organization.PhoneNumber;
        org.Name = organization.Name;

        await _data.SaveChangesAsync();
    }
}