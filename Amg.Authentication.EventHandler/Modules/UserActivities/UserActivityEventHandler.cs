using System.Threading.Tasks;
using Amg.Authentication.Application.Events.UserActivities;
using Amg.Authentication.DomainModel.Modules.UserActivities.Interfaces;
using Amg.Authentication.EventHandler.Mappings.UserActivities;
using Amg.Authentication.Infrastructure.Base;
using MassTransit;

namespace Amg.Authentication.EventHandler.Modules.UserActivities
{
    public class UserActivityEventHandler :
        IConsumer<UserCodeResentEvent>,
        IConsumer<UserCodeVerifiedEvent>,
        IConsumer<UserPasswordChangedEvent>,
        IConsumer<UserPasswordForgetRequestedEvent>,
        IConsumer<UserProfileUpdatedEvent>,
        IConsumer<UserRegisteredEvent>,
        IConsumer<UserRolesChangedEvent>,
        IConsumer<UserSignedInEvent>,
        IConsumer<UserStatusChangedEvent>,
        IConsumer<UserOtpStatusChangedEvent>
    {
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserActivityEventHandler(IUserActivityRepository userActivityRepository,
            IUnitOfWork unitOfWork)
        {
            _userActivityRepository = userActivityRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<UserCodeResentEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserCodeVerifiedEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserPasswordChangedEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserPasswordForgetRequestedEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserProfileUpdatedEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserRolesChangedEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserSignedInEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserStatusChangedEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<UserOtpStatusChangedEvent> context)
        {
            var userActivity = context.Message.ToModel();
            await _userActivityRepository.AddAsync(userActivity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
