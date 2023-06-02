using Volunteasy.Core.DTOs;

namespace Volunteasy.Application;

public interface IAuthenticator
{
    
    // SignUp creates an user and returns their new ID and auth token
    public Task<(string, string)> SignUp(long userId, UserCredentials identification);

    // SignIn logs the user in and returns their ID and auth token
    public Task<(string, string)> SignIn(UserCredentials identification);

    public Task RemoveUserByExternalId(string ext);
}