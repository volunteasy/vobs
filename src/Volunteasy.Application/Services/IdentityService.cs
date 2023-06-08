using Microsoft.Extensions.Logging;
using Volunteasy.Core.Data;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly Data _data;

    private readonly IAuthenticator _authenticator;

    private readonly ILogger<IdentityService> _log;

    public IdentityService(Data data, IAuthenticator authenticator, ILogger<IdentityService> log)
    {
        _data = data;
        _authenticator = authenticator;
        _log = log;
    }

    public async Task<UserToken> AuthenticateUser(UserCredentials identification) => 
        new() { Token = await _authenticator.SignIn(identification) };
}