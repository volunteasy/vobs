using System.Security.Claims;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IIdentityService
{
    Task<string> AuthenticateUser(UserCredentials identification);

    Task<List<ClaimsIdentity>> GetUserSessionClaims(long userId);
}