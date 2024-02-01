using System;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Infrastructure.CrossCutting
{
    public class InMemoryCommandBus : ICommandBus
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryCommandBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommandBase
        {
            var handler = GetCommandHandler<TCommand>();
            await handler.HandleAsync(command);
        }

        /// <inheritdoc />
        public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command) where TCommand : ICommandBase
        {
            var handler = GetCommandHandler<TCommand, TResult>();
            return await handler.HandleAsync(command);
        }


        #region InternalMethods

        private ICommandHandler<TCommand> GetCommandHandler<TCommand>() where TCommand : ICommandBase
        {
            var handler = _serviceProvider.GetService(typeof(ICommandHandler<TCommand>));
            if (handler == null)
                throw new InvalidOperationException("No command executor registered for command type " + typeof(TCommand).FullName);

            return (ICommandHandler<TCommand>)handler;
        }

        private ICommandHandler<TCommand, TResult> GetCommandHandler<TCommand, TResult>() where TCommand : ICommandBase
        {
            var handler = _serviceProvider.GetService(typeof(ICommandHandler<TCommand, TResult>));
            if (handler == null)
                throw new InvalidOperationException("No command executor registered for command type " + typeof(TCommand).FullName);

            return (ICommandHandler<TCommand, TResult>)handler;
        }

        #endregion
    }

}
