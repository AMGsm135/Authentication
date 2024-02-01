using EventEnums = Amg.Authentication.Application.Events.Enums;
using AppEnums = Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.EventHandler.Mappings
{
    public static class EventEnumMappers
    {
        public static AppEnums.UserActivities.SignInType ToAppEnum(this EventEnums.SignInType @enum) =>
            (AppEnums.UserActivities.SignInType)@enum;
        
        public static AppEnums.UserActivities.ChangePasswordType ToAppEnum(this EventEnums.ChangePasswordType @enum) =>
            (AppEnums.UserActivities.ChangePasswordType)@enum;

        public static AppEnums.UserActivities.SmsCodeType ToAppEnum(this EventEnums.SmsCodeType @enum) =>
            (AppEnums.UserActivities.SmsCodeType)@enum;

        public static AppEnums.SignInResultType ToAppEnum(this EventEnums.SignInResultType @enum) =>
            (AppEnums.SignInResultType)@enum;

        public static AppEnums.PersonType ToAppEnum(this EventEnums.PersonType @enum) =>
            (AppEnums.PersonType)@enum;

    }
}
