using System;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Shared;

namespace Amg.Authentication.Application.Contract.Dtos
{
    public class SignInResult
    {
        public Guid? UserId { get; set; }
        public SignInResultType Result { get; set; }
        public UserTicket Ticket { get; set; }
        public string AccessToken { get; set; }
        public object AdditionalData { get; set; }

        public static SignInResult FromResult(SignInResultType resultType, Guid? userId = null)
        {
            return new SignInResult()
            {
                Result = resultType,
                AccessToken = null,
                Ticket = null,
                UserId = userId,
                AdditionalData = null
            };
        }

        public static SignInResult FromResult(SignInResultType resultType, Guid userId, object additionalData)
        {
            return new SignInResult()
            {
                Result = resultType,
                AccessToken = null,
                Ticket = null,
                UserId = userId,
                AdditionalData = additionalData
            };
        }

        public bool IsSuccess =>
            Result == SignInResultType.LoginSuccessful ||
            Result == SignInResultType.ActivationNeeded ||
            Result == SignInResultType.TwoFactorCodeNeeded;
    }

}
