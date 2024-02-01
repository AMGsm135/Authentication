using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Exceptions;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Events.UserActivities;
using Amg.Authentication.Command.Accounting.Passwords;
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
    public class PasswordsCommandHandler : ICommandHandler,
        ICommandHandler<ForgetPasswordCommand, Guid>,
        ICommandHandler<ResetPasswordCommand>,
        ICommandHandler<DefaultPasswordCommand>,
        ICommandHandler<RestorePasswordCommand>,
        ICommandHandler<ChangePasswordCommand>,
        ICommandHandler<ResetPasswordByAdminCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthSettings _authSettings;
        private readonly ISignInService _signInService;
        private readonly IClientInfoGrabber _clientInfoGrabber;
        private readonly IBusControl _bus;


        public PasswordsCommandHandler(IOptions<AuthSettings> authSettings, UserManager<User> userManager,
            IClientInfoGrabber clientInfoGrabber, ISignInService signInService, IBusControl bus)
        {
            _userManager = userManager;
            _clientInfoGrabber = clientInfoGrabber;
            _authSettings = authSettings.Value;
            _signInService = signInService;
            _bus = bus;
        }

        public async Task<Guid> HandleAsync(ForgetPasswordCommand command)
        {
            var user = await _userManager.FindByNameAsync(command.UserName);

            // به خاطر مسائل امنیتی باید یک ای دی فیک برگردانیم
            if (user == null)
                return Guid.NewGuid();

            await _signInService.GenerateAndSendPasswordResetCode(user);

            await _bus.Publish(new UserPasswordForgetRequestedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = true,
            });

            return user.Id;
        }


        public async Task HandleAsync(ResetPasswordCommand command)
        {
            var user = await GetUser(command.UserId, false);

            // به خاطر مسائل امنیتی ما به کاربر اعلام نمیکنیم که آیا این یوزر در سیستم وجود دارد یا خیر
            if (user == null)
                return;

            var isValid = await _signInService.VerifyResetPasswordCode(user, command.Code);
            IdentityResult passwordChangeResult = default;
            if (isValid)
            {
                user.OldPassword = user.PasswordHash;
                await _userManager.UpdateAsync(user);
                await _userManager.RemovePasswordAsync(user);
                passwordChangeResult = await _userManager.AddPasswordAsync(user, command.NewPassword);
            }

            await SendChangePasswordEvent(user.Id, isValid && passwordChangeResult.Succeeded, ChangePasswordType.ResetByUser);

            if (!isValid)
                throw new ServiceException("کد تاییدیه نامعتبر است");

            if (!passwordChangeResult.Succeeded)
                throw new ServiceException(passwordChangeResult.Errors?.FirstOrDefault()?.Description);
        }


        public async Task HandleAsync(DefaultPasswordCommand command)
        {
            var user = await GetUser(command.UserId);

            user.OldPassword = user.PasswordHash;
            await _userManager.UpdateAsync(user);

            var defaultPassword = _authSettings.Password.DefaultPassword;
            await _userManager.RemovePasswordAsync(user);
            var passwordChangeResult = await _userManager.AddPasswordAsync(user, defaultPassword);

            await SendChangePasswordEvent(user.Id, passwordChangeResult.Succeeded, ChangePasswordType.SetToDefaultByAdmin);

            if (!passwordChangeResult.Succeeded)
                throw new ServiceException(passwordChangeResult.Errors?.FirstOrDefault()?.Description);
        }


        public async Task HandleAsync(RestorePasswordCommand command)
        {
            var user = await GetUser(command.UserId);

            if (string.IsNullOrEmpty(user.OldPassword))
            {
                await SendChangePasswordEvent(user.Id, false, ChangePasswordType.RestoreByAdmin);
                throw new ServiceException("بازنشانی رمز عبور کاربر امکان پذیر نیست.");
            }

            user.PasswordHash = user.OldPassword;
            var result = await _userManager.UpdateAsync(user);

            await SendChangePasswordEvent(user.Id, result.Succeeded, ChangePasswordType.RestoreByAdmin);
        }


        public async Task HandleAsync(ChangePasswordCommand command)
        {
            var user = await GetUser(command.UserId);

            var check = await _userManager.CheckPasswordAsync(user, command.CurrentPassword);
            if (!check)
            {
                await SendChangePasswordEvent(user.Id, false, ChangePasswordType.ChangeByUser);
                throw new ServiceException("رمز عبور فعلی اشتباه می باشد.");
            }

            var currentPassword = user.PasswordHash;
            var result = await _userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
            if (!result.Succeeded)
            {
                await SendChangePasswordEvent(user.Id, false, ChangePasswordType.ChangeByUser);
                throw new ServiceException(result.Errors?.FirstOrDefault()?.Description);
            }

            user.OldPassword = currentPassword;
            await _userManager.UpdateAsync(user);
            await SendChangePasswordEvent(user.Id, true, ChangePasswordType.ChangeByUser);
        }

        public async Task HandleAsync(ResetPasswordByAdminCommand command)
        {
            var user = await GetUser(command.UserId);

            user.OldPassword = user.PasswordHash;
            await _userManager.UpdateAsync(user);

            var newPassword = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "").Replace("+", "")
                .TrimEnd('=')[..12];

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, newPassword);
            await SendChangePasswordEvent(user.Id, result.Succeeded, ChangePasswordType.ResetByAdmin);

            if (result.Succeeded)
                await _signInService.SendNewPassword(user, newPassword);
            else
                throw new ServiceException(result.Errors?.FirstOrDefault()?.Description);
        }



        private async Task SendChangePasswordEvent(Guid userId, bool isSuccess, ChangePasswordType type)
        {
            await _bus.Publish(new UserPasswordChangedEvent()
            {
                UserId = userId,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = isSuccess,
                Type = type.ToEventEnum(),
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
