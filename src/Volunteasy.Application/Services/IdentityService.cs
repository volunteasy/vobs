using Volunteasy.Core.DTOs;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class IdentityService : IIdentityService
{

    private readonly IAuthenticator _authenticator;

    public IdentityService(IAuthenticator authenticator)
    {
        _authenticator = authenticator;
    }

    public async Task<string> AuthenticateUser(UserCredentials identification) => 
        await _authenticator.SignIn(identification);
}