using System.Security.Claims;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Services;
using static System.Enum;

namespace Volunteasy.Api.Context;

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
            var value = _context?.Request.RouteValues["organizationId"];
            if (value != null) return ConvertId(value, "OrganizationId");
            
            _log.LogWarning("Organization Id could not be retrieved from route");
            throw new InvalidValuesException("organizationId", value);
        }
    }
    
    public string OrganizationSlug
    {
        get
        {
            var value = _context?.Request.RouteValues["organizationSlug"];
            if (value != null) return value.ToString() ?? "";
            
            _log.LogWarning("Organization slug could not be retrieved from route");
            throw new InvalidValuesException("organizationSlug", value);

        }
    }

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