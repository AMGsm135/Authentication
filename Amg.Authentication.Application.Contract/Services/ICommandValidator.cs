using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface ICommandValidator : IApplicationService
    {
        public void Validate(ICommandBase command);
    }
}
