using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;
using static System.Enum;

namespace Volunteasy.WebApp;

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

    private long ConvertId(object? val, string prmName)
    {
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

class CM : CookieAuthenticationHandler
{
    private readonly IBeneficiaryService _identity;

    public CM(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock, IBeneficiaryService identity) : base(options, logger, encoder, clock)
    {
        _identity = identity;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return base.HandleAuthenticateAsync();
    }

    protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    {
        if ((!(properties?.Parameters.TryGetValue("document", out var cred) ?? false)) || cred == null)
            throw new ArgumentException("Missing credential");
        
        var beneficiary = await _identity.GetBeneficiaryByDocumentAndBirthDate((BeneficiaryKey)cred);

        user.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, beneficiary.Id.ToString()),
            new(ClaimTypes.Name, beneficiary.Name),
            new(ClaimTypes.Email, beneficiary.Email)
        }, CookieAuthenticationDefaults.AuthenticationScheme));

        await base.HandleSignInAsync(user, properties);
    }
}


public class AuthHandler : CookieAuthenticationHandler
{
    private readonly IBeneficiaryService _identity;
    
    public AuthHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IBeneficiaryService identity) : base(options, logger, encoder, clock)
    {
        _identity = identity;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        
        var result = await base.HandleAuthenticateAsync();

        if (result.Succeeded)
            return result;

        var key = new BeneficiaryKey();

        if (Context.Request.Form.TryGetValue("document", out var doc))
        {
            key.Document = doc.ToString();
        }
        
        if (Context.Request.Form.TryGetValue("birthDate", out var birthDate))
        {
            key.BirthDate = DateTime.Parse(birthDate.ToString());
        }

        if (string.IsNullOrEmpty(key.Document) || string.IsNullOrEmpty(birthDate))
        {
            return result;
        }

        try
        {
            var beneficiary = await _identity.GetBeneficiaryByDocumentAndBirthDate(key);

            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, beneficiary.Id.ToString()),
                new(ClaimTypes.Name, beneficiary.Name),
                new(ClaimTypes.Email, beneficiary.Email)
            }, CookieAuthenticationDefaults.AuthenticationScheme)), null, CookieAuthenticationDefaults.AuthenticationScheme));
        }
        catch (Exception e)
        {
            return AuthenticateResult.Fail(e);
        }
    }
}