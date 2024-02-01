using System.Threading.Tasks;

namespace Amg.Authentication.Infrastructure.Base
{
    public interface ICommandBus
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommandBase;

        Task<TResult> SendAsync<TCommand, TResult>(TCommand command) where TCommand : ICommandBase;
    }
}
