using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Application.Contract.Requests;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface ISignInService : IApplicationService
    {
        Task<SignInResult> PasswordSignIn(SignInByPasswordRequest command);
        Task<SignInResult> PhoneNumberSignIn(SignInByPhoneNumberRequest command);
        Task<SignInResult> ExternalProviderSignIn(SignInByExternalProviderRequest command, bool userWasCreatedBeforOrNot);
        Task<SignInResult> TwoFactorSignIn(SignInByTwoFactorCodeRequest command);
        Task<SignInResult> RefreshSignIn();


        Task GenerateAndSendActivationCode(User user);
        Task<(bool isSuccess, string message)> GenerateAndSendConfirmRegisterWithPhoneNumberCode(string phoneNumber);
        Task<bool> VerifyActivationCode(User user, string code);

        Task GenerateAndSendTwoFactorCode(User user);
        Task<bool> VerifyTwoFactorCode(User user, string code);

        Task GenerateAndSendPasswordResetCode(User user);
        Task<bool> VerifyResetPasswordCode(User user, string code);

        Task SendNewPassword(User user, string password);


        bool AccountActivationNeeded(User user);
        bool IsTwoFactorChannelAvailable(User user, TwoFactorType type);

        Task<bool> IsTokenValid(Guid tokenId, Guid userId);
        Task<List<UserTokenItem>> GetActiveSessions(Guid userId);
        Task SignOut(Guid tokenId);
        Task SignOut();
    }
}