using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Application.Contract.Exceptions;
using Amg.Authentication.Application.Contract.Requests;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Events.UserActivities;
using Amg.Authentication.Application.Mappers;
using Amg.Authentication.Application.Services.CashServices;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Enums.UserActivities;
using Amg.Authentication.Infrastructure.Helpers;
using Amg.Authentication.Infrastructure.Settings;
using Amg.Authentication.Infrastructure.Shared;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static MassTransit.ValidationResultExtensions;
using static Org.BouncyCastle.Math.EC.ECCurve;
using SignInResult = Amg.Authentication.Application.Contract.Dtos.SignInResult;

namespace Amg.Authentication.Application.Services
{
    public class SignInService : /*LogServices<SignInByPhoneNumberRequest>,*/ ISignInService
    {
        private const string ActivationPurpose = "Activation";
        private const string TwoFactorPurpose = "TwoFactor";
        private const string ResetPasswordPurpose = "ResetPassword";
        private const string ConfirmRegisterWithPhoneNumberPurpose = "ConfirmRegisterWithPhoneNumber";

        private readonly UserManager<User> _userManager;
        private readonly AuthSettings _authSettings;
        private readonly HostSettings _hostSettings;
        private readonly NotificationSettings _notificationSettings;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IJwtFactory _jwtFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientInfoGrabber _clientInfoGrabber;
        private readonly ITokenManager _tokenManager;
        private readonly IBusControl _bus;
        private readonly IOtpAuthenticatorService _otpAuthenticatorService;
        private readonly ICacheService _cacheService;

        public SignInService(UserManager<User> userManager, IOptions<AuthSettings> authSettings, IOptions<NotificationSettings> notificationSetting, IOptions<HostSettings> hostSetting,
            ISmsSender smsSender, IJwtFactory jwtFactory, IHttpContextAccessor httpContextAccessor,
            IClientInfoGrabber clientInfoGrabber, ITokenManager tokenManager, IBusControl bus,
            IEmailSender emailSender, IOtpAuthenticatorService otpAuthenticatorService, ICacheService cacheService, ILogger<SignInByPhoneNumberRequest> logger, IServiceProvider serviceProvider) /*: base(logger, serviceProvider)*/
        {
            _userManager = userManager;
            _authSettings = authSettings.Value;
            _notificationSettings = notificationSetting.Value;
            _smsSender = smsSender;
            _jwtFactory = jwtFactory;
            _httpContextAccessor = httpContextAccessor;
            _clientInfoGrabber = clientInfoGrabber;
            _bus = bus;
            _emailSender = emailSender;
            _otpAuthenticatorService = otpAuthenticatorService;
            _tokenManager = tokenManager;
            _cacheService = cacheService;
            _hostSettings = hostSetting.Value;
        }

        public async Task<SignInResult> PasswordSignIn(SignInByPasswordRequest command)
        {
            // بررسی نام کاربری
            var user = await _userManager.FindByNameAsync(command.UserName);
            if (user == null)
                return SignInResult.FromResult(SignInResultType.InvalidCredentials);

            // بررسی رمز عبور
            var check = await _userManager.CheckPasswordAsync(user, command.Password);
            if (!check)
            {
                if (_authSettings.Lockout.Enabled)
                    await _userManager.AccessFailedAsync(user);

                await SendSignInEvent(user.Id, false, SignInType.ByPassword, SignInResultType.InvalidCredentials);
                return SignInResult.FromResult(SignInResultType.InvalidCredentials);
            }

            // بررسی فعال بودن کاربر
            if (!user.IsActive)
            {
                await SendSignInEvent(user.Id, false, SignInType.ByPassword, SignInResultType.UserIsInactive);
                return SignInResult.FromResult(SignInResultType.UserIsInactive);
            }

            // بررسی مسدود بودن کاربر
            if (_authSettings.Lockout.Enabled && user.LockoutEnabled &&
                user.LockoutEnd != null && user.LockoutEnd.Value < DateTimeOffset.Now)
            {
                await SendSignInEvent(user.Id, false, SignInType.ByPassword, SignInResultType.UserIsLockedOut);
                return SignInResult.FromResult(SignInResultType.UserIsLockedOut);
            }

            // بررسی تعداد ورود همزمان مجاز
            var sessions = await _tokenManager.GetTokensByUserId(user.Id);
            if (sessions.Count >= _authSettings.Token.MaxActiveSessions)
            {
                return SignInResult.FromResult(SignInResultType.MaxActiveSessionsReached, user.Id, sessions);
            }

            // بررسی فعال بودن حساب کاربری
            if (AccountActivationNeeded(user))
            {
                await GenerateAndSendActivationCode(user);
                await SendSignInEvent(user.Id, false, SignInType.ByPassword, SignInResultType.ActivationNeeded);
                return SignInResult.FromResult(SignInResultType.ActivationNeeded, user.Id);
            }

            // بررسی فعال بودن کد تایید دو عاملی
            if (user.TwoFactorEnabled)
            {
                await GenerateAndSendTwoFactorCode(user);
                await SendSignInEvent(user.Id, false, SignInType.ByPassword, SignInResultType.TwoFactorCodeNeeded);
                return SignInResult.FromResult(SignInResultType.TwoFactorCodeNeeded, user.Id);
            }

            // ورود کاربر
            await SendSignInEvent(user.Id, true, SignInType.ByPassword, SignInResultType.LoginSuccessful);
            var roles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));
            return await SignInByToken(user, roles);
        }

        public async Task<SignInResult> PhoneNumberSignIn(SignInByPhoneNumberRequest command)
        {
            /// آیا وضیعت توسعه روشن است
            //if (!_notificationSettings.Sms.DevelopmentMode)
            //{
            // بررسی تطابق کد ها
            //var isValid = await _cacheService.GetData<string>(command.PhoneNumber) == command.Code;
            //if (!isValid)
            //return SignInResult.FromResult(SignInResultType.OneTimePasswordInvalid);
            //}

            // اگر وضیعت توسعه روشن است کد با کد پی فرض باید برابر باشد
            //if (_notificationSettings.Sms.DevelopmentMode && _notificationSettings.Sms.DevelopmentCode != command.Code)
            //{
            //return SignInResult.FromResult(SignInResultType.OneTimePasswordInvalid);
            //}

            // بررسی وجود کاربر
            var user = _userManager.Users.SingleOrDefault(x => x.PhoneNumber == command.PhoneNumber);
            if (user is null)
                return SignInResult.FromResult(SignInResultType.InvalidRequest);

            // بررسی فعال بودن کاربر
            if (!user.IsActive)
            {
                await SendSignInEvent(user.Id, false, SignInType.ByPhoneNumber, SignInResultType.UserIsInactive);
                return SignInResult.FromResult(SignInResultType.UserIsInactive);
            }

            // بررسی مسدود بودن کاربر
            if (_authSettings.Lockout.Enabled && user.LockoutEnabled &&
                user.LockoutEnd != null && user.LockoutEnd.Value < DateTimeOffset.Now)
            {
                await SendSignInEvent(user.Id, false, SignInType.ByPhoneNumber, SignInResultType.UserIsLockedOut);
                return SignInResult.FromResult(SignInResultType.UserIsLockedOut);
            }

            // بررسی تعداد ورود همزمان مجاز
            var sessions = await _tokenManager.GetTokensByUserId(user.Id);
            if (sessions.Count >= _authSettings.Token.MaxActiveSessions)
            {
                return SignInResult.FromResult(SignInResultType.MaxActiveSessionsReached, user.Id, sessions);
            }

            // ورود کاربر
            await SendSignInEvent(user.Id, true, SignInType.ByPhoneNumber, SignInResultType.LoginSuccessful);

            var roles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);

            //Log(new EventId(201, "PhoneNumberSignIn"), $"Successfully Login For User with phoneNumber {command.PhoneNumber}", LogEnumType.SuccessLog);
            var result = await SignInByToken(user, roles);
            return result;

        }



        public async Task<SignInResult> ExternalProviderSignIn(SignInByExternalProviderRequest command, bool userWasCreatedBeforOrNot)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);

            if (userWasCreatedBeforOrNot)
            {
                // بررسی فعال بودن کاربر
                if (!user.IsActive)
                {
                    await SendSignInEvent(user.Id, false, SignInType.ByPhoneNumber, SignInResultType.UserIsInactive);
                    return SignInResult.FromResult(SignInResultType.UserIsInactive);
                }

                // بررسی مسدود بودن کاربر
                if (_authSettings.Lockout.Enabled && user.LockoutEnabled &&
                    user.LockoutEnd != null && user.LockoutEnd.Value < DateTimeOffset.Now)
                {
                    await SendSignInEvent(user.Id, false, SignInType.ByPhoneNumber, SignInResultType.UserIsLockedOut);
                    return SignInResult.FromResult(SignInResultType.UserIsLockedOut);
                }

                // بررسی تعداد ورود همزمان مجاز
                var sessions = await _tokenManager.GetTokensByUserId(user.Id);
                if (sessions.Count >= _authSettings.Token.MaxActiveSessions)
                {
                    return SignInResult.FromResult(SignInResultType.MaxActiveSessionsReached, user.Id, sessions);
                }

                // بررسی فعال بودن حساب کاربری
                if (AccountActivationNeeded(user))
                {
                    await GenerateAndSendActivationCode(user);
                    await SendSignInEvent(user.Id, false, SignInType.ByPhoneNumber, SignInResultType.ActivationNeeded);
                    return SignInResult.FromResult(SignInResultType.ActivationNeeded, user.Id);
                }

                // ورود کاربر
                await SendSignInEvent(user.Id, true, SignInType.ByPhoneNumber, SignInResultType.LoginSuccessful);
                var roles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));
                return await SignInByToken(user, roles);
            }
            else
            {
                // ورود کاربر
                await SendSignInEvent(user.Id, true, SignInType.ByExternalProvider, SignInResultType.LoginSuccessful);
                var roles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));
                return await SignInByToken(user, roles);
            }

        }

        public async Task<SignInResult> TwoFactorSignIn(SignInByTwoFactorCodeRequest command)
        {
            // بررسی نام کاربری
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user == null)
                return SignInResult.FromResult(SignInResultType.InvalidCredentials);

            // بررسی فعال بودن کاربر
            if (!user.IsActive)
            {
                await SendSignInEvent(user.Id, false, SignInType.ByTwoFactorCode, SignInResultType.UserIsInactive);
                return SignInResult.FromResult(SignInResultType.UserIsInactive);
            }

            // بررسی مسدود بودن کاربر
            if (_authSettings.Lockout.Enabled && user.LockoutEnabled &&
                user.LockoutEnd != null && user.LockoutEnd.Value < DateTimeOffset.Now)
            {
                await SendSignInEvent(user.Id, false, SignInType.ByTwoFactorCode, SignInResultType.UserIsLockedOut);
                return SignInResult.FromResult(SignInResultType.UserIsLockedOut);
            }

            // بررسی تعداد ورود همزمان مجاز
            var sessions = await _tokenManager.GetTokensByUserId(user.Id);
            if (sessions.Count >= _authSettings.Token.MaxActiveSessions)
            {
                return SignInResult.FromResult(SignInResultType.MaxActiveSessionsReached, user.Id, sessions);
            }

            // بررسی فعال بودن کد تایید دو عاملی
            var roles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));
            if (!user.TwoFactorEnabled)
            {
                await SendSignInEvent(user.Id, false, SignInType.ByTwoFactorCode, SignInResultType.InvalidRequest);
                return SignInResult.FromResult(SignInResultType.InvalidRequest);
            }

            // بررسی کد تایید دو عاملی
            var isVerified = await VerifyTwoFactorCode(user, command.VerifyCode);
            if (!isVerified)
            {
                await _userManager.AccessFailedAsync(user);
                await SendSignInEvent(user.Id, false, SignInType.ByTwoFactorCode, SignInResultType.TwoFactorCodeInvalid);
                return SignInResult.FromResult(SignInResultType.TwoFactorCodeInvalid);
            }

            await SendSignInEvent(user.Id, false, SignInType.ByTwoFactorCode, SignInResultType.LoginSuccessful);
            // ورود کاربر
            return await SignInByToken(user, roles);
        }

        public async Task<SignInResult> RefreshSignIn()
        {
            var ticket = GetUserInfoFromRequest();
            if (ticket == null)
                return SignInResult.FromResult(SignInResultType.InvalidCredentials);

            // بررسی توکن فعلی
            if (!await IsTokenValid(ticket.TokenId, ticket.UserId))
                return SignInResult.FromResult(SignInResultType.InvalidCredentials);

            // حذف توکن فعلی
            await _tokenManager.RemoveToken(ticket.TokenId, ticket.UserId);

            // بررسی تعداد ورود همزمان مجاز
            var sessions = await _tokenManager.GetTokensByUserId(ticket.UserId);
            if (sessions.Count >= _authSettings.Token.MaxActiveSessions)
            {
                return SignInResult.FromResult(SignInResultType.MaxActiveSessionsReached, ticket.UserId, sessions);
            }

            // بررسی نام کاربری
            var user = await _userManager.FindByIdAsync(ticket.UserId.ToString());
            if (user == null)
                return SignInResult.FromResult(SignInResultType.InvalidCredentials);

            // بررسی فعال بودن کاربر
            if (!user.IsActive)
                return SignInResult.FromResult(SignInResultType.UserIsInactive);

            // بررسی فعال بودن حساب کاربری
            if (AccountActivationNeeded(user))
                return SignInResult.FromResult(SignInResultType.InvalidRequest, user.Id);

            var roles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));

            // ورود کاربر، تولید و ثبت توکن
            return await SignInByToken(user, roles);
        }




        public async Task GenerateAndSendActivationCode(User user)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:
                    var smsCode = await _userManager.GenerateUserTokenAsync(user, Constants.CustomPhoneTokenProvider, ActivationPurpose);
                    await _smsSender.SendSmsAsync(user.PhoneNumber, SmsMessages.ActivationCode(smsCode));
                    return;
                case VerifyAccountType.Email:
                    var emailCode = await _userManager.GenerateUserTokenAsync(user,
                        TokenOptions.DefaultEmailProvider, ActivationPurpose);
                    await _emailSender.SendEmailAsync(user.Email, EmailMessages.ActivationCodeSubject(), EmailMessages.ActivationCodeBody(emailCode));
                    return;
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {_authSettings.VerifyAccountType.ToString()}");
            }
        }


        public async Task<bool> VerifyActivationCode(User user, string code)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:
                    return await _userManager.VerifyUserTokenAsync(user,
                        TokenOptions.DefaultPhoneProvider, ActivationPurpose, code);
                case VerifyAccountType.Email:
                    return await _userManager.VerifyUserTokenAsync(user,
                        TokenOptions.DefaultEmailProvider, ActivationPurpose, code);
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {_authSettings.VerifyAccountType.ToString()}");
            }
        }
        /// <summary>
        /// ارسال پیامک و یا ایمیل برای تایید شماره همراه یا ایمیل
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<(bool isSuccess, string message)> GenerateAndSendConfirmRegisterWithPhoneNumberCode(string phoneNumber)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:

                    var smsCode = CodeGenerator.Generate();
                    var resultSmsSender = await _smsSender.SendSmsAsyncWithResult(phoneNumber, smsCode);

                    if (!resultSmsSender.isSuccess)
                        return (false, resultSmsSender.message);

                    return (true, string.Empty);

                default:
                    throw new NotImplementedException($"این نوع ثبت نام هنوز در دسترس نمی باشد لطفا از روش دیگری امتحان کنید . {_authSettings.VerifyAccountType.ToString()}");
            }
        }

        public async Task<bool> VerifyConfirmRegisterWithPhoneNumberCode(User user, string code)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:
                    return await _userManager.VerifyUserTokenAsync(user,
                        TokenOptions.DefaultPhoneProvider, ConfirmRegisterWithPhoneNumberPurpose, code);
                case VerifyAccountType.Email:
                    return await _userManager.VerifyUserTokenAsync(user,
                        TokenOptions.DefaultEmailProvider, ConfirmRegisterWithPhoneNumberPurpose, code);
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {_authSettings.VerifyAccountType.ToString()}");
            }
        }

        public async Task GenerateAndSendTwoFactorCode(User user)
        {
            foreach (var twoFactorType in _authSettings.TwoFactorTypes.OrderBy(i => i.Order))
            {
                if (!IsTwoFactorChannelAvailable(user, twoFactorType.Type))
                    continue;

                switch (twoFactorType.Type)
                {
                    case TwoFactorType.Sms:
                        var smsCode = await _userManager.GenerateUserTokenAsync(user,
                            TokenOptions.DefaultPhoneProvider, TwoFactorPurpose);
                        await _smsSender.SendSmsAsync(user.PhoneNumber, SmsMessages.TwoFactorCode(smsCode));
                        return;
                    case TwoFactorType.Email:
                        // به دلیل عدم استفاده از لینک، برای کد دومرحله ای هم از توکن پروایدر تلفن استفاده می شود.
                        var emailCode = await _userManager.GenerateUserTokenAsync(user,
                            TokenOptions.DefaultPhoneProvider, TwoFactorPurpose);
                        await _emailSender.SendEmailAsync(user.Email, EmailMessages.TwoFactorCodeSubject(), EmailMessages.TwoFactorCodeBody(emailCode));
                        return;
                    case TwoFactorType.OtpCode:
                        // نیازی به ارسال کد نیست و کاربر از طریق
                        // Authenticator
                        // کد را دریافت می کند.
                        return;
                    default:
                        throw new NotImplementedException($"This type of authentication is not implemented yet : {twoFactorType.Type.ToString()}");
                }
            }
            throw new ServiceException($"No two factor channels available for this user.");
        }

        public async Task<bool> VerifyTwoFactorCode(User user, string code)
        {
            foreach (var twoFactorType in _authSettings.TwoFactorTypes.OrderBy(i => i.Order))
            {
                if (!IsTwoFactorChannelAvailable(user, twoFactorType.Type))
                    continue;

                switch (twoFactorType.Type)
                {
                    case TwoFactorType.Sms:
                        return await _userManager.VerifyUserTokenAsync(user,
                            TokenOptions.DefaultPhoneProvider, TwoFactorPurpose, code);
                    case TwoFactorType.Email:
                        // به دلیل عدم استفاده از لینک، برای کد دومرحله ای هم از توکن پروایدر تلفن استفاده می شود.
                        return await _userManager.VerifyUserTokenAsync(user,
                            TokenOptions.DefaultPhoneProvider, TwoFactorPurpose, code);
                    case TwoFactorType.OtpCode:
                        return _otpAuthenticatorService.ValidateTwoFactorPin(user.OtpSecretCode, code);
                    default:
                        throw new NotImplementedException($"This type of authentication is not implemented yet : {twoFactorType.Type.ToString()}");
                }
            }
            throw new ServiceException($"No two factor channels available for this user.");
        }

        public async Task GenerateAndSendPasswordResetCode(User user)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:
                    var smsCode = await _userManager.GenerateUserTokenAsync(user,
                        TokenOptions.DefaultPhoneProvider, ResetPasswordPurpose);
                    await _smsSender.SendSmsAsync(user.PhoneNumber, SmsMessages.ForgetPasswordCode(smsCode));
                    break;
                case VerifyAccountType.Email:
                    var emailCode = await _userManager.GenerateUserTokenAsync(user,
                        TokenOptions.DefaultEmailProvider, ResetPasswordPurpose);
                    await _emailSender.SendEmailAsync(user.Email, EmailMessages.ForgetPasswordCodeSubject(), EmailMessages.ForgetPasswordCodeBody(emailCode));
                    break;
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {_authSettings.VerifyAccountType.ToString()}");
            }
        }

        public async Task<bool> VerifyResetPasswordCode(User user, string code)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:
                    return await _userManager.VerifyUserTokenAsync(user,
                        TokenOptions.DefaultPhoneProvider, ResetPasswordPurpose, code);
                case VerifyAccountType.Email:
                    return await _userManager.VerifyUserTokenAsync(user,
                        TokenOptions.DefaultEmailProvider, ResetPasswordPurpose, code);
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {_authSettings.VerifyAccountType.ToString()}");
            }
        }

        public async Task SendNewPassword(User user, string password)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:
                    await _smsSender.SendSmsAsync(user.PhoneNumber, SmsMessages.ResetPasswordByAdmin(password));
                    break;
                case VerifyAccountType.Email:
                    await _emailSender.SendEmailAsync(user.Email, EmailMessages.ResetPasswordByAdminSubject(), EmailMessages.ResetPasswordByAdminBody(password));
                    break;
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {_authSettings.VerifyAccountType.ToString()}");
            }
        }


        public bool AccountActivationNeeded(User user)
        {
            switch (_authSettings.VerifyAccountType)
            {
                case VerifyAccountType.Sms:
                    return !user.PhoneNumberConfirmed;
                case VerifyAccountType.Email:
                    return !user.EmailConfirmed;
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {_authSettings.VerifyAccountType.ToString()}");
            }
        }

       

        public bool IsTwoFactorChannelAvailable(User user, TwoFactorType type)
        {
            if (!user.TwoFactorEnabled)
                return false;

            switch (type)
            {
                case TwoFactorType.Sms:
                    return user.PhoneNumberConfirmed;
                case TwoFactorType.Email:
                    return user.EmailConfirmed;
                case TwoFactorType.OtpCode:
                    return user.OtpEnabled;
                default:
                    throw new NotImplementedException($"This type of authentication is not implemented yet : {type.ToString()}");
            }
        }

        public async Task<bool> IsTokenValid(Guid tokenId, Guid userId)
        {
            var userToken = await _tokenManager.GetToken(tokenId, userId);
            if (userToken == null)
                return false;

            var clientInfo = _clientInfoGrabber.GetClientInfo();
            if (_authSettings.Token.RevokeOnIpChange &&
                userToken.ClientInfo.IP != clientInfo.IP)
                return false;

            if (_authSettings.Token.RevokeOnClientChange &&
                (userToken.ClientInfo.Agent != clientInfo.Agent ||
                 userToken.ClientInfo.Device != clientInfo.Device ||
                 userToken.ClientInfo.OS != clientInfo.OS))
                return false;

            return true;
        }

        public async Task<List<UserTokenItem>> GetActiveSessions(Guid userId)
        {
            return await _tokenManager.GetTokensByUserId(userId);
        }

        public async Task SignOut()
        {
            var ticket = GetUserInfoFromRequest();
            if (ticket == null)
                return;

            await _tokenManager.RemoveToken(ticket.TokenId, ticket.UserId);
        }

        public async Task SignOut(Guid tokenId)
        {
            await _tokenManager.RemoveToken(tokenId);
        }

        /// <summary>
        /// دریافت اطلاعات توکن از درخواست جاری
        /// <para />
        /// به دلیل این که توکن ممکن است منقضی شده باشد اطلاعات کاربر را بدون اعتبار سنجی زمان انقضا دریافت می کنیم.
        /// </summary>
        /// <returns></returns>
        private UserTicket GetUserInfoFromRequest()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return null;
            var ticket = _jwtFactory.DecodeToken(token, false);

            if (ticket != null && ticket.RefreshExpireAt < DateTime.Now)
                return null;

            return ticket;
        }

        private async Task<SignInResult> SignInByToken(User user, List<RoleType> roles)
        {
            var userTicket = new UserTicket()
            {
                TokenId = Guid.NewGuid(),
                UserId = user.Id,
                Roles = roles,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Name = $"{user.FirstName} {user.LastName}",
                PersonType = user.PersonType,
                TokenExpireAt = DateTime.Now.Add(TimeSpan.FromMinutes(_authSettings.Token.TokenLifeTime)),
                RefreshExpireAt = DateTime.Now.Add(TimeSpan.FromMinutes(_authSettings.Token.RefreshTimeout))
            };

            var token = _jwtFactory.GenerateToken(userTicket);

            await _tokenManager.AddToken(new UserTokenItem()
            {
                UserId = userTicket.UserId,
                TokenId = userTicket.TokenId,
                ClientInfo = _clientInfoGrabber.GetClientInfo()
            }, TimeSpan.FromMinutes(_authSettings.Token.RefreshTimeout));

            return new SignInResult()
            {
                UserId = user.Id,
                AccessToken = token,
                Ticket = userTicket,
                Result = SignInResultType.LoginSuccessful,
            };

        }

        private async Task SendSignInEvent(Guid userId, bool isSuccess, SignInType type, SignInResultType resultType)
        {
            await _bus.Publish(new UserSignedInEvent()
            {
                UserId = userId,
                SignInType = type.ToEventEnum(),
                ResultType = resultType.ToEventEnum(),
                IsSuccess = isSuccess,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
            });
        }

        private async Task SendRegisterEvent(Guid userId, bool isSuccess, string phoneNumber, PersonType personType)
        {

            await _bus.Publish(new UserRegisteredEvent()
            {
                UserId = userId,
                PhoneNumber = phoneNumber,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                PersonType = personType.ToEventEnum(),
                IsSuccess = isSuccess,
                ByAdmin = false,
            });
        }

    }
}
