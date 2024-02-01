using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface IClientInfoGrabber : IApplicationService
    {
        ClientInfo GetClientInfo();
    }
}
