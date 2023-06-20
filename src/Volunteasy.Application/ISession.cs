using Volunteasy.Core.Enums;

namespace Volunteasy.Application;

public interface ISession
{
    long UserId { get; }
    
    long OrganizationId { get; }
    
    bool CanAccessAs(params MembershipRole[] wantedRole)
        => wantedRole.Length != 0 && wantedRole.Contains(
            CurrentRole());
    bool IsOwner() => CanAccessAs(MembershipRole.Owner);
    
    MembershipRole CurrentRole();
    
    bool IsUser(long userId) => userId == UserId;
}