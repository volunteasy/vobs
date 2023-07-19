using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class BeneficiaryService : ServiceBase, IBeneficiaryService
{
    protected BeneficiaryService(Data data, IVolunteasyContext session) : base(data, session) { }
    
    public async Task<Beneficiary> CreateBeneficiary(BeneficiaryCreation creation)
    {
        try
        {
            var beneficiary = creation.ToBeneficiary();
            beneficiary.OrganizationId = Session.OrganizationId;
            beneficiary.Active = true;
            
            Data.Beneficiaries.Add(beneficiary);
            await Data.SaveChangesAsync();

            return beneficiary;
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateResourceException(typeof(Beneficiary));
        }
    }

    public async Task UpdateBeneficiary(long id, BeneficiaryCreation edition)
    {
        try
        {
            var beneficiary = await Data.Beneficiaries
                .WithOrganization(Session.OrganizationId)
                .SingleOrDefaultAsync(b => b.Id == id);

            if (beneficiary == null)
                throw new ResourceNotFoundException(typeof(Beneficiary));

            beneficiary.Name = edition.Name;
            beneficiary.Document = edition.Document;
            beneficiary.Phone = edition.Phone;
            beneficiary.Email = edition.Email;
            beneficiary.BirthDate = edition.BirthDate.Date;
            beneficiary.Address = edition.Address;

            await Data.SaveChangesAsync();
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateResourceException(typeof(Beneficiary));
        }

    }

    public async Task DisableBeneficiaryById(long id)
    {
        var beneficiary = await Data.Beneficiaries
            .WithOrganization(Session.OrganizationId)
            .SingleOrDefaultAsync(b => b.Id == id);

        if (beneficiary == null)
            throw new ResourceNotFoundException(typeof(Beneficiary));

        beneficiary.Active = false;
        await Data.SaveChangesAsync();
    }

    public async Task<PaginatedList<BeneficiaryResume>> ListBeneficiaries(BeneficiaryFilter filter, long token)
    {
        return await Data.Beneficiaries
            .WithOrganization(filter.OrganizationId)
            .WithPageToken(token)
            .WithFilters(
                new (filter.Document != null, b => b.Document.Contains(filter.Document ?? "")),
                new (filter.Name != null, b => b.Name.Contains(filter.Name ?? "")),
                new (filter.Phone != null, b => b.Phone.Contains(filter.Phone ?? ""))
            ).Include(b => b.Address)
            .Select(b => new BeneficiaryResume
            {
                Id = b.Id,
                OrganizationId = b.OrganizationId,
                Name = b.Name,
                Document = b.Document,
                Phone = b.Phone,
                Email = b.Email,
                BirthDate = b.BirthDate,
                MemberSince = b.MemberSince,
                Address = b.Address,
                Active = b.Active
            })
            .OrderBy(b => b.Name)
            .PaginateList(b => b.Id);
    }

    public async Task<BeneficiaryResume> GetBeneficiaryById(long id)
    {
        var beneficiary = await Data.Beneficiaries
            .WithOrganization(Session.OrganizationId)
            .Select(b => new BeneficiaryResume
            {
                Id = b.Id,
                OrganizationId = b.OrganizationId,
                Name = b.Name,
                Document = b.Document,
                Phone = b.Phone,
                Email = b.Email,
                BirthDate = b.BirthDate,
                MemberSince = b.MemberSince,
                Address = b.Address,
                Active = b.Active
            })
            .SingleOrDefaultAsync(b => b.Id == id);

        if (beneficiary == null)
            throw new ResourceNotFoundException(typeof(Beneficiary));

        return beneficiary;
    }

    public async Task<Beneficiary> GetBeneficiaryByDocumentAndBirthDate(BeneficiaryKey key)
    {
        var beneficiary = await Data.Beneficiaries
            .WithOrganization(Session.OrganizationId)
            .SingleOrDefaultAsync(b => b.Document == key.Document && b.BirthDate == key.BirthDate);

        if (beneficiary == null)
            throw new ResourceNotFoundException(typeof(Beneficiary));

        return beneficiary;
    }

    
}