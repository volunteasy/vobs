using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IUserService
{
    Task<User> CreateUser(UserRegistration registration);
    
    public Task<User> GetUserById(long id);

    public Task UpdateUser(long id, UserDetails identification);
}