using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Shared;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface IJwtFactory : IApplicationService
    {
        string GenerateToken(UserTicket userTicket);
        UserTicket DecodeToken(string token, bool validateLifeTime);
    }

}
