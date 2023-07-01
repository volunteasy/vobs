using Microsoft.AspNetCore.Mvc.Filters;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using ISession = Volunteasy.Core.Services.ISession;

namespace Volunteasy.Api.Context;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
{
    private readonly MembershipRole[] _roles;

    public AuthorizeRolesAttribute(params MembershipRole[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = (ISession) new Session(context.HttpContext);

        if (!session.CanAccessAs(_roles))
            throw new UserNotAuthorizedException();
    }
}