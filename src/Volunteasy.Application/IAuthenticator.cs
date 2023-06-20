using Volunteasy.Core.Model;

namespace Volunteasy.Application;

public interface IAuthenticator
{
    
    // SignUp creates an user and returns their new ID and auth token
    public Task<string> SignUp(long userId, UserCredentials identification);

    // SignIn logs the user in and returns their ID and auth token
    public Task<string> SignIn(UserCredentials identification);
}