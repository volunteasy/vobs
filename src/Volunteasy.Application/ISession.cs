namespace Volunteasy.Application;

public interface ISession
{
    public long UserId { get; }
    public string ExternalId { get; }
}