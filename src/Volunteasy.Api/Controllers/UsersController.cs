using Microsoft.AspNetCore.Mvc;

namespace Volunteasy.Api.Controllers;

public class UsersController : Controller
{
    // GET
    public IActionResult GetUser()
    {
        return Ok(HttpContext.User);
    }
}