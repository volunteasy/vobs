using System.Security.Claims;
using ISession = Volunteasy.Application.ISession;

namespace Volunteasy.Api.Context;

public class Session : ISession
{
    private readonly HttpContext _context;

    public Session(HttpContext context)
    {
        _context = context;
    }


    public string ExternalId => _context.User.Identity?.Name ?? "";
    
    public bool IsAuthenticated => _context.User.Identity?.IsAuthenticated ?? false;

    public long Id => Convert.ToInt64(_context.User.FindFirst(ClaimTypes.Name)?.Value);
}