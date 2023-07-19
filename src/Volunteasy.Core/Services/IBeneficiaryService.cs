using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IBeneficiaryService
{
    public Task<Beneficiary> CreateBeneficiary(BeneficiaryCreation creation);

    public Task UpdateBeneficiary(long id, BeneficiaryCreation edition);

    public Task DisableBeneficiaryById(long id);

    public Task<PaginatedList<BeneficiaryResume>> ListBeneficiaries(BeneficiaryFilter filter, long token);

    public Task<BeneficiaryResume> GetBeneficiaryById(long id);

    public Task<Beneficiary> GetBeneficiaryByDocumentAndBirthDate(BeneficiaryKey key);
}