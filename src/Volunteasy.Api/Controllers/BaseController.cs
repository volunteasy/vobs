using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Volunteasy.Core.DTOs;
using Volunteasy.Api.Response;

namespace Volunteasy.Api.Controllers;

public class BaseController : Controller
{
    protected Response.Response Payload => new Response.Response { Context = HttpContext };

    [NonAction]
    protected IActionResult PaginatedList<TK>(IEnumerable<TK> list, Filter listFilter, bool hasNext = false)
    {
        var nextPage = !hasNext ? "": $"start={listFilter.NextPage.Item1}&end={listFilter.NextPage.Item2}";
        
        new List<KeyValuePair<string, StringValues>>
        {
            new("X-Has-Next", hasNext.ToString().ToLower()),
            new("X-Next-Page", nextPage)
        }.ForEach(HttpContext.Response.Headers.Add);

        return Ok(Payload with
        {
            Data = new
            {
                HasNext = hasNext,
                NextPage = nextPage,
                Items = list
            }
        });
    }
}