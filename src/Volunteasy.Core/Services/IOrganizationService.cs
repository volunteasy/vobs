using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IOrganizationService
{
    public Task<Organization> CreateOrganization(OrganizationRegistration org);
    public Task<PaginatedList<OrganizationDetails>> ListOrganizations(OrganizationFilter filter, long pageToken);
    public Task<OrganizationDetails> GetOrganizationById(long id);
    public Task UpdateOrganizationById(long id, OrganizationRegistration org);
}