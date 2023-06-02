using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IUserService
{
    public Task<User> GetUserById(long id);

    public Task UpdateUser(long id, UpdateUserDataRequest identification);
}