using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Api.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : Controller
{
    private readonly IIdentityService _identities;
    public AuthController(IIdentityService identities)
    {
        _identities = identities;
    }
    
    [HttpPost]
    public async Task<IActionResult> SignUp(UserIdentification identification)
    {
        try
        {
            var res = await _identities.SignUp(identification);
            
            if (res.Data?.Token != null)
                HttpContext.Response.Headers.Authorization = res.Data.Token;
            
            return StatusCode((int)res.Status);
        }
        catch (Exception e)
        {
            var res = Response<User>.UnhandledError(e.Message);
            return StatusCode((int)res.Status, res);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> SignIn(UserCredentials identification)
    {
        try
        {
            var res = await _identities.SignIn(identification);

            if (res.Data?.Token != null)
                HttpContext.Response.Headers.Authorization = res.Data.Token;
            
            return StatusCode((int)res.Status);
        }
        catch (Exception e)
        {
            var res = Response<User>.UnhandledError(e.Message);
            return StatusCode((int)res.Status, res);
        }
    }

    [HttpGet]
    public IActionResult AmIConnected()
    {
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            
        }
        foreach (var userClaim in HttpContext.User.Claims)
        {
            Console.Write(userClaim +"\n");
        }
        
        return Ok();
    }
}