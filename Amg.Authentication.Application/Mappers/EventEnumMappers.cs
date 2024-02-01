using Amg.Authentication.Application.Events.Enums;

namespace Amg.Authentication.Application.Mappers
{
    public static class EventEnumMappers
    {
        public static SignInType ToEventEnum(this Infrastructure.Enums.UserActivities.SignInType @enum) =>
            (SignInType)@enum;
        
        public static ChangePasswordType ToEventEnum(this Infrastructure.Enums.UserActivities.ChangePasswordType @enum) =>
            (ChangePasswordType)@enum;

        public static SmsCodeType ToEventEnum(this Infrastructure.Enums.UserActivities.SmsCodeType @enum) =>
            (SmsCodeType)@enum;

        public static SignInResultType ToEventEnum(this Infrastructure.Enums.SignInResultType @enum) =>
            (SignInResultType)@enum;

        public static PersonType ToEventEnum(this Infrastructure.Enums.PersonType @enum) =>
            (PersonType)@enum;

    }
}
