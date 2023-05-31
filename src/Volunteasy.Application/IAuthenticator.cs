using System.Security.Claims;
using System.Security.Principal;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;

namespace Volunteasy.Application;

public interface IAuthenticator
{
    
    // SignUp creates an user and returns their new ID and auth token
    public Task<(string, string)> SignUp(UserCredentials identification);

    // SignIn logs the user in and returns their ID and auth token
    public Task<(string, string)> SignIn(UserCredentials identification);

    public Task<ClaimsPrincipal?> GetClaimsByToken(string token);
}