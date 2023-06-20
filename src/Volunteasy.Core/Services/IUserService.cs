using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IUserService
{
    Task<User> CreateUser(UserRegistration registration, bool shallow = false);
    
    public Task<User> GetUserById(long id);

    public Task UpdateUser(long id, UserDetails identification);
}