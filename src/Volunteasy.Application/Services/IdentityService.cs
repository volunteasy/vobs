using System.Security.Claims;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
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

    public async Task<UserResume> AuthenticateUser(UserCredentials identification)
    {
        var token = await _authenticator.SignIn(identification);

        var user = await _data.Users
            .SingleOrDefaultAsync(u => u.Email == identification.Email);

        if (user == null)
            throw new UserNotFoundException();

        var memberships = await _data.Memberships
            .Where(x => x.MemberId == user.Id)
            .Where(x => x.Status == MembershipStatus.Approved)
            .Join(_data.Organizations,
                m => m.OrganizationId, o => o.Id, (membership, org) => new OrganizationMember
                {
                    Role = membership.Role,
                    Status = membership.Status,
                    MemberSince = membership.MemberSince,
                    OrganizationId = membership.OrganizationId,
                    MemberId = membership.MemberId,
                    OrganizationName = org.Name,
                    NextDistributionsNumber =
                        _data.Distributions.Count(x =>
                            x.OrganizationId == org.Id && x.StartsAt >= DateTime.Now.ToUniversalTime()),
                    DistributionsNumber =
                        _data.Distributions.Count(x => x.OrganizationId == org.Id && !x.Canceled),
                    MembershipsNumber =
                        _data.Memberships.Count(x => x.OrganizationId == org.Id && x.Role == MembershipRole.Assisted)
                }).ToListAsync();

        return new UserResume
        {
            Id = user.Id,
            Name = user.Name!,
            Email = user.Email!,
            Memberships = memberships,
            Token = token
        };
    }
        

    public async Task<List<ClaimsIdentity>> GetUserSessionClaims(long userId)
    {
        
        var organizations = await _data.Memberships
            .Where(x => x.MemberId == userId)
            .Where(x => x.Status == MembershipStatus.Approved)
            .Join(_data.Organizations, m => m.OrganizationId, o => o.Id, 
                (membership, organization) => new { Membership = membership, Organization = organization })
            .Select(m => new ClaimsIdentity(new OrganizationMemberIdentity
            {
                Name = m.Membership.OrganizationId.ToString(),
                AuthenticationType = m.Membership.Role.ToString(),
                OrganizationName = m.Organization.Name,
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
    
    public string? OrganizationName { get; set; }
    public string? Name { get; set; }
}