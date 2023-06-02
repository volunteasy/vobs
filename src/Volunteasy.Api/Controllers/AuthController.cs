using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : BaseController<AuthController>
{
    private readonly IIdentityService _identities;
    public AuthController(IIdentityService identities, ILogger<AuthController> log) : base(log)
    {
        _identities = identities;
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register new user",
        Description = "Registers and authenticates a new user. A token will be returned in the Authorization header"
    )]
    public async Task<IActionResult> RegisterUser(UserIdentification identification)
    {
        try
        {
            var res = await _identities.RegisterUser(identification);
            
            if (res.Token != null)
                HttpContext.Response.Headers.Authorization = res.Token;
            
            return Created(res.UserId.ToString(), null);
        }
        catch (Exception e)
        {
            return Error(e);
        }
    }
    
    [HttpPut]
    [SwaggerOperation(
        Summary = "Authenticate user",
        Description = "Authenticates an user. A token will be returned in the Authorization header"
    )]
    public async Task<IActionResult> AuthenticateUser(UserCredentials identification)
    {
        try
        {
            var res = await _identities.AuthenticateUser(identification);

            if (res.Token != null)
                HttpContext.Response.Headers.Authorization = res.Token;
            
            return NoContent();
        }
        catch (Exception e)
        {
            return Error(e);
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Validate authentication",
        Description = "Authentication healthcheck. Verifies if a token is valid and authorizes the request"
    )]
    [Authorize]
    public IActionResult AmIConnected()
    {
        return Ok(new
        {
            Message = "You're successfully connected"
        });
    }
}