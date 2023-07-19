using Volunteasy.Core.Data;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class ServiceBase
{
    protected readonly Data Data;

    protected readonly IVolunteasyContext Session;

    protected ServiceBase(Data data, IVolunteasyContext session)
    {
        Data = data;
        Session = session;
    }
}