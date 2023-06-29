using Volunteasy.Core.Data;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class ServiceBase
{
    protected readonly Data Data;

    protected readonly ISession Session;

    protected ServiceBase(Data data, ISession session)
    {
        Data = data;
        Session = session;
    }
}