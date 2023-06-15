using System.Security.Claims;
using Volunteasy.Core.DTOs;

namespace Volunteasy.Core.Services;

public interface IIdentityService
{
    Task<string> AuthenticateUser(UserCredentials identification);

    Task<List<ClaimsIdentity>> GetUserSessionClaims(long userId);
}