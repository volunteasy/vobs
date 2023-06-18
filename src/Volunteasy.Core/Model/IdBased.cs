namespace Volunteasy.Core.Model;

public class IdBased
{
    public long Id { get; set; }
}

public class OrganizationBasedResource : IdBased
{
    public long OrganizationId { get; set; }
}