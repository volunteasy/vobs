using Volunteasy.Core.Enums;

namespace Volunteasy.Core.Services;

public interface IVolunteasyContext
{
    long UserId { get; }
    
    long OrganizationId { get; }
    
    MembershipRole CurrentRole { get; }
    
    
    
    bool IsOwner() => CanAccessAs(MembershipRole.Owner);
    
    bool IsUser(long userId) => userId == UserId;
    
    bool CanAccessAs(params MembershipRole[] wantedRole)
        => wantedRole.Length != 0 && wantedRole.Contains(
            CurrentRole);
}