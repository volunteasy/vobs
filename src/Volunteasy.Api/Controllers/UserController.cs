using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/user")]
public class UserController : BaseController
{
    private readonly IUserService _users;

    private readonly IMembershipService _memberships;

    private readonly IBenefitService _benefits;
    public UserController(IUserService users, IMembershipService memberships, IBenefitService benefits)
    {
        _users = users;
        _memberships = memberships;
        _benefits = benefits;
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
    
    [HttpGet("{userId:long}/memberships")]
    public async Task<IActionResult> ListUserMemberships([FromQuery] MembershipFilter filter, long userId, long pageToken)
    {
        var (members, next) = await _memberships.ListMemberships(
            filter with { MemberId = userId }, pageToken);
        return PaginatedList(members, next);
    }
    
    [HttpGet("{userId:long}/benefits")]
    public async Task<IActionResult> ListUserBenefits([FromQuery] BenefitFilter filter, long userId, long pageToken)
    {
        var (members, next) = await _benefits.ListBenefits(
            filter with { AssistedId = userId }, pageToken);
        return PaginatedList(members, next);
    }
}