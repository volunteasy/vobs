using Volunteasy.Core.DTOs;

namespace Volunteasy.Core.Services;

public interface IIdentityService
{
    Task<UserToken> AuthenticateUser(UserCredentials identification);
}