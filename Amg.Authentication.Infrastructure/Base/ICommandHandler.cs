using System.Threading.Tasks;

namespace Amg.Authentication.Infrastructure.Base
{
    public interface ICommandHandler
    {
        // base interface for scanning CommandHandlers
    }


    public interface ICommandHandler<in TCommand> where TCommand : ICommandBase
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommandBase
    {
        Task<TResult> HandleAsync(TCommand command);
    }

}