using System.Security.Claims;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;
using static System.Enum;

namespace Volunteasy.App;

public class VolunteasyContext : IVolunteasyContext
{
    private readonly HttpContext? _context;

    private readonly ILogger<VolunteasyContext> _log;

    public VolunteasyContext(IHttpContextAccessor context, ILogger<VolunteasyContext> log)
    {
        _log = log;
        _context = context.HttpContext;
    }

    public long UserId
    {
        get
        {
            // Get UserId from JWT claims
            var value = _context?.User.FindFirst("volunteasy_id")?.Value;
            if (value != null) return ConvertId(value, "UserId");

            // Get UserId from Cookie claims
            value = _context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (value != null) return ConvertId(value, "UserId");
            
            _log.LogWarning("User Id could not be retrieve from HttpContext");
            throw new InvalidValuesException("organizationId", value);
        }
    }

    public long OrganizationId
    {
        get
        {

            if ((_context?.Items.TryGetValue("organization", out var org) ?? false) && org != null)
                return ((Organization)org).Id;

            _log.LogWarning("Organization Id could not be retrieved from context");
            throw new InvalidValuesException("organization", "");
        }
    }
    
    public string OrganizationSlug => _context?.GetRouteValue("orgSlug")?.ToString() ?? "";

    public MembershipRole CurrentRole
    {
        get
        {
            var orgAuthentication = _context?.User.Identities
                .Where(identity => identity.Name == OrganizationId.ToString())
                .Select(identity => identity.AuthenticationType ?? "")
                .SingleOrDefault();

            return string.IsNullOrEmpty(orgAuthentication) ? 0 : Parse<MembershipRole>(orgAuthentication);
        }
    }
    
    private long ConvertId(object? val, string prmName) {
        try
        {
            return Convert.ToInt64(val);
        }
        catch (Exception e)
        {
            _log.LogWarning(e, "{PrmName} from HttContext could not be parsed", prmName);
            throw new InvalidValuesException(prmName, val);
        }
    }
}