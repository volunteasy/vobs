using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Volunteasy.Core.DTOs;

namespace Volunteasy.Api.Controllers;

public class BaseController<T> : Controller
{
    private readonly ILogger<BaseController<T>> _log;
    protected Response Payload => new Response { Context = HttpContext };

    public BaseController(ILogger<BaseController<T>> log)
    {
        _log = log;
    }

    [NonAction]
    protected IActionResult Error(Exception e)
    {
        if (e is ApplicationException)
        {
            var status = Convert.ToInt16(e.HelpLink);
            if (status > 0)
            {
                _log.LogWarning(e, "Captured expected error");
                return StatusCode(status, Payload with
                {
                    Reason = e.GetType().Name,
                    Message = e.Message
                });
            }
        }

        _log.LogError(e, "Captured unexpected error");
        return StatusCode(500, Payload with
        {
            Reason = "UnexpectedError",
            Message = "Ops! Um erro inesperado ocorreu. Tente novamente mais tarde",
        });
    }
    
    [NonAction]
    protected IActionResult PaginatedList<TK>(IEnumerator<TK> list, Filter listFilter, bool hasNext = false)
    {
        new List<KeyValuePair<string, StringValues>>
        {
            new("X-Has-Next", hasNext.ToString().ToLower()),
            new("X-Next-Page", $"start={listFilter.NextPage.Item1}&end=start={listFilter.NextPage.Item1}")
        }.ForEach(HttpContext.Response.Headers.Add);

        return Ok(Payload with
        {
            Data = new
            {
                HasNext = hasNext.ToString().ToLower(),
                NextPage = $"start={listFilter.NextPage.Item1}&end=start={listFilter.NextPage.Item1}",
                Items = list
            }
        });
    }
}

public record Response
{
    [JsonIgnore]
    public HttpContext Context { get; init; } = null!;

    public int Status => Context.Response.StatusCode;

    public bool Success => Context.Response.StatusCode is >= 200 and <= 299;
    
    public string CorrelationId => Context.TraceIdentifier;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reason { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; init; }
}