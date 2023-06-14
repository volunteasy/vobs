using System.ComponentModel.DataAnnotations;
using Volunteasy.Core.Enums;

namespace Volunteasy.Core.Model;

public class Membership
{
    [Required]
    public long OrganizationId { get; set; }
    
    public Organization? Organization { get; set; }
    
    [Required]
    public long MemberId { get; set; }
    
    [Required]
    public MembershipRole Role { get; set; }
    
    [Required]
    public MembershipStatus Status { get; set; }
    
    [Required]
    public DateTime MemberSince { get; set; }
}