using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : BaseController
{
    private readonly IIdentityService _identities;
    public AuthController(IIdentityService identities)
    {
        _identities = identities;
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Authenticate user",
        Description = "Authenticates an user. A token will be returned in the Authorization header"
    )]
    public async Task<IActionResult> AuthenticateUser(UserCredentials identification)
    {
        var token = await _identities.AuthenticateUser(identification);

        HttpContext.Response.Headers.Authorization = token;
            
        return NoContent();
    }
}