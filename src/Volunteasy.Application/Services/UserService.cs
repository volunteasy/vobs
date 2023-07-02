using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volunteasy.Core.Data;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class UserService : IUserService
{
    private readonly Data _data;

    private readonly ISession _session;

    private readonly ILogger<UserService> _log;

    private readonly IAuthenticator _authenticator;

    public UserService(Data data, IAuthenticator authenticator, ISession session, ILogger<UserService> log)
    {
        _data = data;
        _session = session;
        _log = log;
        _authenticator = authenticator;
    }
    
    public async Task<User> CreateUser(UserRegistration registration, bool shallow = false)
    {
        var user = _data.Add(new User
        {
            Document = registration.Document,
            Name = registration.Name,
            Email = registration.Email,
            Address = registration.Address,
            PhoneAddress = registration.PhoneAddress,
        });

        try
        {
            await _data.SaveChangesAsync();

            if (shallow) return user.Entity;
            
            var  externalId = await _authenticator.SignUp(user.Entity.Id, new UserCredentials
            {
                Email = registration.Email,
                Password = registration.Password
            });

            user.Entity.ExternalId = externalId;
            await _data.SaveChangesAsync();

            return user.Entity;
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateUserException();
        }
        catch (Exception e)
        {
            _log.LogInformation(e, "failed creating user: rolling it back");
            _data.Users.Remove(user.Entity);
            
            #pragma warning disable CS4014
            _data.SaveChangesAsync();
            #pragma warning restore CS4014
            
            throw;
        }
    }
    
    public async Task<User> GetUserById(long id)
    {
        var user = await _data.Users.SingleOrDefaultAsync(u => u.Id.Equals(id));
        return user switch
        {
            null => throw new UserNotFoundException(), _ => user
        };
    }

    public async Task UpdateUser(long id, UserDetails identification)
    {
        if (id != _session.UserId)
            throw new UserNotAuthorizedException();
        
        var user = await _data.Users.SingleOrDefaultAsync(u => u.Id.Equals(id));
        if (user is null)
            throw new UserNotFoundException();

        if (user.Email != identification.Email)
            // TODO: Send e-mail confirmation link
            _log.LogInformation("Ignoring e-mail update for now as it is not yet implemented");
        
        user.Document = identification.Document;
        user.Name = identification.Name;

        try
        {
            await _data.SaveChangesAsync();
        }
        catch (UniqueConstraintException)
        {
            throw new DuplicateUserException();
        }

        
    }
}