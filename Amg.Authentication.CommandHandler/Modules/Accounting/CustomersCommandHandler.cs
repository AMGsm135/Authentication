using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Exceptions;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Events.UserActivities;
using Amg.Authentication.Application.Services.CashServices;
using Amg.Authentication.Command.Accounting.Customers;
using Amg.Authentication.CommandHandler.Mappers;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Enums.UserActivities;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Amg.Authentication.CommandHandler.Modules.Accounting
{
    public class CustomersCommandHandler : /*StorageLogService<RegisterCustomerCommand, int>,*/ ICommandHandler,
        ICommandHandler<RegisterCustomerCommand>,
        ICommandHandler<RegisterCustomerWithPhoneNumberCommand, TimeSpan>,
        ICommandHandler<UpdateCustomerPhoneNumberCommand>,
        ICommandHandler<UpdateCustomerCommand>,
        ICommandHandler<ResendActivationCodeCommand>,
        ICommandHandler<OTPResendActivationCodeCommand, TimeSpan>,
        ICommandHandler<VerifyActivationCodeCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly ISignInService _signInService;
        private readonly IClientInfoGrabber _clientInfoGrabber;
        private readonly IBusControl _bus;
        private readonly ICacheService _cacheService;


        public CustomersCommandHandler(UserManager<User> userManager, ISignInService signInService,
            IClientInfoGrabber clientInfoGrabber, IBusControl bus, ICacheService cacheService, /*ILogger<RegisterCustomerCommand> logger,*/ IServiceProvider serviceProvider) /*: base(logger, serviceProvider, 100)*/
        {
            _userManager = userManager;
            _signInService = signInService;
            _clientInfoGrabber = clientInfoGrabber;
            _bus = bus;
            _cacheService = cacheService;
        }

        public async Task HandleAsync(RegisterCustomerCommand command)
        {
            var currentCustomer = await _userManager.FindByNameAsync(command.UserName);
            if (currentCustomer != null)
            {
                //LogAdd("نام کاربری وارد شده تکراری می باشد", new ServiceException("نام کاربری وارد شده تکراری می باشد"));
                throw new ServiceException("نام کاربری وارد شده تکراری می باشد");
            }
            var newCustomer = new User(command.UserName, command.Name, command.PersonType, null)
            {
                Id = command.Id,
                PhoneNumber = command.PhoneNumber,
                NormalizedUserName = command.UserName.ToUpper(),
                PhoneNumberConfirmed = false,
                Email = command.Email,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
            };
            var createResult = await _userManager.CreateAsync(newCustomer, command.Password);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newCustomer, RoleType.Customer.ToString());

                await _signInService.GenerateAndSendActivationCode(newCustomer);
                //Successfully Add To Database
                //LogAdd();
                await _bus.Publish(new UserRegisteredEvent()
                {
                    UserId = newCustomer.Id,
                    Name = newCustomer.Name,
                    Email = newCustomer.Email,
                    PhoneNumber = newCustomer.PhoneNumber,
                    ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                    PersonType = newCustomer.PersonType.ToEventEnum(),
                    IsSuccess = true,
                    ByAdmin = false,
                });
            }
            else
            {
                //LogAdd(ex: new ServiceException(createResult.Errors?.FirstOrDefault()?.Description));
                throw new ServiceException(createResult.Errors?.FirstOrDefault()?.Description);
            }
        }

        /// <summary>
        /// ثبت نام  با شماره همراه
        ///نکته : تا زمانی که دریافت کد صورت نگیرد و تایید نشود کاربر وارد دیتا بیس نمیشود 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public async Task<TimeSpan> HandleAsync(RegisterCustomerWithPhoneNumberCommand command)
        {
            if (_cacheService.ExistsInCache(command.PhoneNumber))
                return _cacheService.GetTimeToLive(command.PhoneNumber);

            var notificationResult = await _signInService.GenerateAndSendConfirmRegisterWithPhoneNumberCode(command.PhoneNumber);

            if (!notificationResult.isSuccess)
            {
                //LogAdd<RegisterCustomerWithPhoneNumberCommand>("sms با موفقیت ارسال نشد");
                throw new ServiceException(notificationResult.message);
            }

            return _cacheService.GetTimeToLive(command.PhoneNumber);
        }




        public async Task HandleAsync(UpdateCustomerPhoneNumberCommand command)
        {
            var user = await GetUser(command.UserId);

            user.PhoneNumber = command.PhoneNumber;
            user.PhoneNumberConfirmed = true;

            var result = await _userManager.UpdateAsync(user);

            await _bus.Publish(new UserProfileUpdatedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = result.Succeeded,
                PhoneNumber = command.PhoneNumber,
            });
        }

        public async Task HandleAsync(UpdateCustomerCommand command)
        {
            var user = await GetUser(command.UserId);

            var nameChanged = user.Name != command.Name;
            var phoneChanged = user.PhoneNumber != command.PhoneNumber;

            user.Name = command.Name;
            user.PhoneNumber = command.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            await _bus.Publish(new UserProfileUpdatedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = result.Succeeded,
                Name = nameChanged ? command.Name : null,
                PhoneNumber = phoneChanged ? user.PhoneNumber : null,
                TwoFactorEnabled = null,
                Email = null,
            });
        }


        public async Task HandleAsync(VerifyActivationCodeCommand command)
        {
            var user = await GetUser(command.UserId, false);

            //به خاطر مسائل امنیتی ما به کاربر اعلام نمیکنیم که آیا این یوزر در سیستم وجود دارد یا خیر
            if (user == null)
                return;

            var isValid = await _signInService.VerifyActivationCode(user, command.Code);

            await _bus.Publish(new UserCodeVerifiedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = isValid,
                CodeType = SmsCodeType.ActivationCode.ToEventEnum(),
                Email = user.Email,
                Mobile = user.PhoneNumber,
                Name = user.Name
            });

            if (!isValid)
                throw new ServiceException("کد وارد شده معتبر نمی باشد.");

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
        }


        public async Task HandleAsync(ResendActivationCodeCommand command)
        {
            var user = await GetUser(command.UserId, false);

            //به خاطر مسائل امنیتی ما به کاربر اعلام نمیکنیم که آیا این یوزر در سیستم وجود دارد یا خیر
            if (user == null)
                return;

            if (user.PhoneNumberConfirmed)
            {
                ///Log<ResendActivationCodeCommand>(new EventId(110, "Resend"), "حساب کاربری هم اکنون فعال می باشد.", LogEnumType.ErrorLog);
                throw new ServiceException("حساب کاربری هم اکنون فعال می باشد.");
            }

            await _signInService.GenerateAndSendActivationCode(user);

            ///Log<ResendActivationCodeCommand>(new EventId(110, "Resend"), "Successfully ResendActivationCodeCommand", LogEnumType.SuccessLog);
            await _bus.Publish(new UserCodeResentEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = true,
                CodeType = SmsCodeType.ActivationCode.ToEventEnum(),
                PhoneNumber = user.PhoneNumber
            });
        }

        public async Task<TimeSpan> HandleAsync(OTPResendActivationCodeCommand command)
        {
            if (_cacheService.ExistsInCache(command.PhoneNumber))
                return _cacheService.GetTimeToLive(command.PhoneNumber);

            var result = await _signInService.GenerateAndSendConfirmRegisterWithPhoneNumberCode(command.PhoneNumber);
   
            if (!result.isSuccess)
                throw new ServiceException(result.message);

            return _cacheService.GetTimeToLive(command.PhoneNumber);
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
