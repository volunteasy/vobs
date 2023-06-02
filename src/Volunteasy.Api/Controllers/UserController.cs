using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/user/{userId:long}")]
public class UserController : BaseController<UserController>
{
    private readonly IUserService _users;
    public UserController(IUserService users, ILogger<UserController> log) : base(log)
    {
        _users = users;
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update user",
        Description = "Updates user data. At the moment, only document and name fields are updated"
    )]
    [Authorize]
    public async Task<IActionResult> UpdatedUser(long userId, UpdateUserDataRequest userIdentification)
    {
        try
        {
            await _users.UpdateUser(userId, userIdentification);
            return NoContent();
        }
        catch (Exception e)
        {
            return Error(e);
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get user by id",
        Description = "Gets an user by its unique identifier"
    )]
    [Authorize]
    public async Task<IActionResult> GetUserById(long userId)
    {
        try
        {
            return Ok(await _users.GetUserById(userId));
        }
        catch (Exception e)
        {
            return Error(e);
        }
    }
}