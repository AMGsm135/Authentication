using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Application.Contract.Exceptions;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Events.UserActivities;
using Amg.Authentication.Command.Accounting.TwoFactors;
using Amg.Authentication.CommandHandler.Mappers;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums.UserActivities;
using Amg.Authentication.Infrastructure.Settings;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Amg.Authentication.CommandHandler.Modules.Accounting
{
    public class TwoFactorsCommandHandler : ICommandHandler,
        ICommandHandler<EnableTwoFactorCommand>,
        ICommandHandler<DisableTwoFactorCommand>,
        ICommandHandler<ResendTwoFactorCodeCommand>,
        ICommandHandler<EnableOtpTwoFactorCommand, OtpActivationResult>,
        ICommandHandler<DisableOtpTwoFactorCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthSettings _authSettings;
        private readonly IClientInfoGrabber _clientInfoGrabber;
        private readonly ISignInService _signInService;
        private readonly IBusControl _bus;
        private readonly IOtpAuthenticatorService _otpAuthenticatorService;

        public TwoFactorsCommandHandler(IOptions<AuthSettings> authSettings, UserManager<User> userManager,
            IClientInfoGrabber clientInfoGrabber, ISignInService signInService, IBusControl bus,
            IOtpAuthenticatorService otpAuthenticatorService)
        {
            _userManager = userManager;
            _authSettings = authSettings.Value;
            _clientInfoGrabber = clientInfoGrabber;
            _signInService = signInService;
            _otpAuthenticatorService = otpAuthenticatorService;
            _bus = bus;
        }

        public async Task HandleAsync(EnableTwoFactorCommand command)
        {
            var user = await GetUser(command.UserId);

            if (user.TwoFactorEnabled)
                throw new ServiceException("رمز عبور دو عاملی هم اکنون برای شما فعال می باشد.");

            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);
        }

        public async Task HandleAsync(DisableTwoFactorCommand command)
        {
            var user = await GetUser(command.UserId);

            if (!user.TwoFactorEnabled)
                throw new ServiceException("رمز عبور دو عاملی برای شما فعال نمی باشد.");

            user.TwoFactorEnabled = false;
            await _userManager.UpdateAsync(user);
        }

        public async Task HandleAsync(ResendTwoFactorCodeCommand command)
        {
            var user = await GetUser(command.UserId, false);

            //به خاطر مسائل امنیتی ما به کاربر اعلام نمیکنیم که آیا این یوزر در سیستم وجود دارد یا خیر
            if (user == null)
                return;

            if (!user.TwoFactorEnabled)
                throw new ServiceException("رمز عبور دو عاملی برای شما فعال نمی باشد.");

            await _signInService.GenerateAndSendTwoFactorCode(user);

            await _bus.Publish(new UserCodeResentEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                CodeType = SmsCodeType.TwoFactorCode.ToEventEnum(),
                IsSuccess = true,
                PhoneNumber = user.PhoneNumber
            });
        }
        
        public async Task<OtpActivationResult> HandleAsync(EnableOtpTwoFactorCommand command)
        {
            var user = await GetUser(command.UserId);

            if (!user.TwoFactorEnabled)
                throw new ServiceException("رمز عبور دو عاملی برای شما فعال نمی باشد.");

            var secret = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "").Replace("+", "")
                .TrimEnd('=')[..16];

            var qrImage = _otpAuthenticatorService.GenerateActivationQrCode(user.UserName, secret);
            user.OtpSecretCode = secret;
            user.OtpEnabled = true;
            await _userManager.UpdateAsync(user);

            await _bus.Publish(new UserOtpStatusChangedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = true,
                IsEnabled = true,
            });

            return new OtpActivationResult()
            {
                QrImage = qrImage,
                SecretCode = secret
            };
        }

        public async Task HandleAsync(DisableOtpTwoFactorCommand command)
        {
            var user = await GetUser(command.UserId);

            if (!user.TwoFactorEnabled && !user.OtpEnabled)
                throw new ServiceException("رمز عبور دو عاملی برای شما فعال نمی باشد.");

            user.OtpEnabled = false;
            await _userManager.UpdateAsync(user);
            await _bus.Publish(new UserOtpStatusChangedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = true,
                IsEnabled = false,
            });
        }





        private async Task<User> GetUser(Guid userId, bool throwIfNotFound = true)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null && throwIfNotFound)
                throw new ServiceException("کاربر یافت نشد");

            return user;
        }


    }
}
