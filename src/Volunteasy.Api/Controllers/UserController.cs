using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/user")]
public class UserController : BaseController
{
    private readonly IUserService _users;
    public UserController(IUserService users)
    {
        _users = users;
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create user",
        Description = "Creates a new user. After that, they can use their credentials to authenticate to our services"
    )]
    public async Task<IActionResult> CreateUser(UserRegistration registration)
    {
        var res = await _users.CreateUser(registration);
        return Created(res.Id.ToString(), null);
    }

    [HttpPut("{userId:long}")]
    [SwaggerOperation(
        Summary = "Update user",
        Description = "Updates user data. At the moment, only document and name fields are updated"
    )]
    [Authorize]
    public async Task<IActionResult> UpdatedUser(long userId, UserDetails userIdentification)
    {
        await _users.UpdateUser(userId, userIdentification);
        return NoContent();
    }

    [HttpGet("{userId:long}")]
    [SwaggerOperation(
        Summary = "Get user by id",
        Description = "Gets an user by its unique identifier"
    )]
    [Authorize]
    public async Task<IActionResult> GetUserById(long userId) =>
        Ok(await _users.GetUserById(userId));
}