using System;
using Amg.Authentication.Application.Events.UserActivities;
using Amg.Authentication.DomainModel.Modules.UserActivities;
using Amg.Authentication.DomainModel.Modules.UserActivities.Activities;
using DomainClientInfo = Amg.Authentication.DomainModel.Modules.UserActivities.ValueObjects.ClientInfo;
using EventClientInfo = Amg.Authentication.Application.Events.UserActivities.Items.ClientInfo;

namespace Amg.Authentication.EventHandler.Mappings.UserActivities
{
    public static class UserActivityMappings
    {
        public static UserActivity ToModel(this UserCodeResentEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new ResendCodeActivity(@event.CodeType.ToAppEnum(), @event.PhoneNumber), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserCodeVerifiedEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new VerifyCodeActivity(@event.CodeType.ToAppEnum()), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserPasswordChangedEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new ChangePasswordActivity(@event.Type.ToAppEnum()), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserPasswordForgetRequestedEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new ForgetPasswordActivity(), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserProfileUpdatedEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new UpdateProfileActivity(@event.Name, @event.PhoneNumber, @event.Email, @event.TwoFactorEnabled),
                @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserRegisteredEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new RegisterUserActivity(@event.Name, @event.PhoneNumber, @event.Email, @event.PersonType.ToAppEnum(),
                    @event.ByAdmin), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserRolesChangedEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new ChangeUserRolesActivity(@event.Role), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserSignedInEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new SignInActivity(@event.SignInType.ToAppEnum(), @event.ResultType.ToAppEnum()), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static UserActivity ToModel(this UserStatusChangedEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new ChangeUserStatusActivity(@event.IsActive), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

        public static DomainClientInfo ToModel(this EventClientInfo clientInfo)
        {
            if (clientInfo == null)
                return DomainClientInfo.Empty();

            return new DomainClientInfo(clientInfo.IP, clientInfo.OS, clientInfo.Device, clientInfo.Agent);
        }

        public static UserActivity ToModel(this UserOtpStatusChangedEvent @event)
        {
            return new UserActivity(@event.UserId, @event.IsSuccess, DateTime.Now,
                new ChangeUserOtpStatusActivity(@event.IsEnabled), @event.ClientInfo.ToModel(), @event.ErrorExceptionMessage);
        }

    }
}
