namespace Volunteasy.Core.Model;

public class IdBased
{
    public long Id { get; set; }
}

public class OrganizationBasedResource : IdBased
{
    public long OrganizationId { get; set; }
}

public interface IOrganization
{
    public long OrganizationId { get; }
}

public interface IId
{
    public long Id { get; }
}

public record PaginatedList<T>(IEnumerable<T> List, string? Next = null);