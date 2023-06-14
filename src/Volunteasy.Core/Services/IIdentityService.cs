using Volunteasy.Core.DTOs;

namespace Volunteasy.Core.Services;

public interface IIdentityService
{
    Task<string> AuthenticateUser(UserCredentials identification);
}