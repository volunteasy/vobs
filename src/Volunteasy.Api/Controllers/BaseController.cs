using Microsoft.AspNetCore.Mvc;
namespace Volunteasy.Api.Controllers;

public class BaseController : Controller
{
    protected Response.Response Payload => new Response.Response { Context = HttpContext };

    [NonAction]
    protected IActionResult PaginatedList<TK>(IEnumerable<TK> list, string? next)
    {
        if (next != null)
            HttpContext.Response.Headers.Add("X-Next-Page-Token", next);

        return Ok(Payload with
        {
            Data = new
            {
                NextPageToken = next,
                Items = list
            }
        });
    }
}