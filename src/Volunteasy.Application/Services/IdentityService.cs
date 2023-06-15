using System.Security.Claims;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class IdentityService : IIdentityService
{

    private readonly IAuthenticator _authenticator;

    private readonly Data _data;

    public IdentityService(Data data, IAuthenticator authenticator)
    {
        _authenticator = authenticator;
        _data = data;
    }

    public async Task<string> AuthenticateUser(UserCredentials identification) => 
        await _authenticator.SignIn(identification);

    public async Task<List<ClaimsIdentity>> GetUserSessionClaims(long userId)
    {
        
        var organizations = await _data.Memberships
            .Where(x => x.MemberId == userId)
            .Where(x => x.Status == MembershipStatus.Approved)
            .Select(m => new ClaimsIdentity(new OrganizationMemberIdentity
            {
                Name = m.OrganizationId.ToString(),
                AuthenticationType = m.Role.ToString(),
                IsAuthenticated = true
            }))
            .ToListAsync();

        return organizations;
    }
}

internal class OrganizationMemberIdentity : IIdentity
{
    public string? AuthenticationType { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Name { get; set; }
}