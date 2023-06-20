using Volunteasy.Core.Data;

namespace Volunteasy.Application.Services;

public class ServiceBase
{
    protected readonly Data Data;

    protected readonly ISession Session;

    public ServiceBase(Data data, ISession session)
    {
        Data = data;
        Session = session;
    }
}