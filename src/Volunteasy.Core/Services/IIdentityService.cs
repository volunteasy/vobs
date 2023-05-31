using Volunteasy.Core.DTOs;

namespace Volunteasy.Core.Services;

public interface IIdentityService
{
    Task<Response<UserToken>> SignUp(UserIdentification identification);
    Task<Response<UserToken>> SignIn(UserCredentials identification);
}