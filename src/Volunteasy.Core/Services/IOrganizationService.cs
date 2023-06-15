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
}