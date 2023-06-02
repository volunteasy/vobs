using System.Security.Claims;
using ISession = Volunteasy.Application.ISession;

namespace Volunteasy.Api.Context;

public class Session : ISession
{
    private readonly HttpContext _context;

    public Session(IHttpContextAccessor context)
    {
        _context = context.HttpContext ?? new DefaultHttpContext();
    }

    public string ExternalId => _context.User.Identity?.Name ?? "";
    
    public bool IsAuthenticated => _context.User.Identity?.IsAuthenticated ?? false;

    public long UserId => Convert.ToInt64(_context.User.FindFirst("volunteasy_id")?.Value);
}