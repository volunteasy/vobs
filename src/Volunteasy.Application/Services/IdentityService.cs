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

    public async Task<UserToken> RegisterUser(UserIdentification identification)
    {
        var user = _data.Add(new User
        {
            Document = identification.Document,
            Name = identification.Name,
            Email = identification.Email
        });
        
        await _data.SaveChangesAsync();

        var res = new UserToken();

        try
        {
            var  (externalId, token) = await _authenticator.SignUp(user.Entity.Id, new UserCredentials
            {
                Email = identification.Email,
                Password = identification.Password
            });

            res = res with { ExternalId = externalId, Token = token};
        }
        catch (Exception)
        {
            _log.LogWarning("failed creating user: rolling it back");
            _data.Users.Remove(user.Entity);
            
            #pragma warning disable CS4014
            _data.SaveChangesAsync();
            #pragma warning restore CS4014
            
            throw;
        }
        
        return res;
    }
    
    public async Task<UserToken> AuthenticateUser(UserCredentials identification)
    {
        var (_, token) = await _authenticator.SignIn(identification);
        return new UserToken { Token = token };
    }
}