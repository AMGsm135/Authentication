using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Amg.Authentication.Infrastructure.Settings;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amg.Authentication.CommandHandler.Modules.Accounting
{
    public class CustomersCommandHandler : /*StorageLogService<RegisterCustomerCommand, int>,*/ ICommandHandler,
        ICommandHandler<RegisterCustomerCommand>,
        ICommandHandler<UpdateCustomerPhoneNumberCommand>,
        ICommandHandler<ChangeCustomerPhoneNumberCommand>,
        ICommandHandler<UpdateCustomerCommand>,
        ICommandHandler<ResendActivationCodeCommand>,
        ICommandHandler<OTPResendActivationCodeCommand, TimeSpan>,
        ICommandHandler<VerifyActivationCodeCommand>,
        ICommandHandler<SendCodeCommand>,
        ICommandHandler<AddCustomerByAdminCommand>,
        ICommandHandler<ApproveCustomerCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly ISignInService _signInService;
        private readonly IClientInfoGrabber _clientInfoGrabber;
        private readonly IBusControl _bus;
        private readonly ICacheService _cacheService;
        private readonly HostSettings _hostSettings;



        public CustomersCommandHandler(UserManager<User> userManager, ISignInService signInService,
            IClientInfoGrabber clientInfoGrabber, IBusControl bus, ICacheService cacheService, IOptions<HostSettings> hostSetting
            /*ILogger<RegisterCustomerCommand> logger,*/) /*: base(logger, serviceProvider, 100)*/
        {
            _userManager = userManager;
            _signInService = signInService;
            _clientInfoGrabber = clientInfoGrabber;
            _bus = bus;
            _cacheService = cacheService;
            _hostSettings = hostSetting.Value;
        }

        public async Task HandleAsync(RegisterCustomerCommand command)
        {
            var currentCustomer = _userManager.Users.SingleOrDefault(i => i.PhoneNumber == command.PhoneNumber);
            if (currentCustomer != null)
                throw new ServiceException("شماره همراه وارد شده تکراری می باشد");

            var newCustomer = new User(command.PhoneNumber, command.FirstName, command.LastName, PersonType.Individual, null, command.City, command.Province)
            {
                Id = command.Id,
                PhoneNumber = command.PhoneNumber,
                NormalizedUserName = null,
                PhoneNumberConfirmed = false,
                Email = null,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
            };
            var createResult = await _userManager.CreateAsync(newCustomer);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newCustomer, RoleType.Customer.ToString());
                await SendCustomerInformation(newCustomer);
                await _bus.Publish(new UserRegisteredEvent()
                {
                    UserId = newCustomer.Id,
                    Name = newCustomer.FirstName + "|" + newCustomer.LastName,
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
                throw new ServiceException(createResult.Errors?.FirstOrDefault()?.Description);
            }
        }

        public async Task HandleAsync(ApproveCustomerCommand command)
        {
            var user = await GetUser(command.Id);

            user.Status = command.IsApproved ? RegisteryStatus.Accepted : RegisteryStatus.Rejected;

            var result = await _userManager.UpdateAsync(user);
            await ApproveCustomer(user.Id, command.IsApproved);
            await _bus.Publish(new UserProfileUpdatedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = result.Succeeded,
                PhoneNumber = user.PhoneNumber,
            });
        }

        private async Task ApproveCustomer(Guid userId, bool isApproved)
        {
            var _client = new HttpClient();

            var approveCommand = new
            {
                Id = userId,
                IsApproved = isApproved,
            };

            var myContent = JsonConvert.SerializeObject(approveCommand);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using HttpResponseMessage response = await _client.PostAsync(_hostSettings.ShopAddress + "/Customer/Approve", byteContent);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ReasonPhrase);
                throw new ServiceException("مشکل در برقراری ارتباط");
            }
        }

        private async Task SendCustomerInformation(User user)
        {
            var _client = new HttpClient();

            var addCustomerCommand = new
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                Province = user.Province,
                PhoneNumber = user.PhoneNumber
            };

            var myContent = JsonConvert.SerializeObject(addCustomerCommand);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using HttpResponseMessage response = await _client.PostAsync(_hostSettings.ShopAddress + "/Customer/Add", byteContent);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ReasonPhrase);
                throw new ServiceException("مشکل در برقراری ارتباط");
            }
        }

        public async Task HandleAsync(AddCustomerByAdminCommand command)
        {
            var currentCustomer = _userManager.Users.SingleOrDefault(i => i.PhoneNumber == command.PhoneNumber);
            if (currentCustomer != null)
                throw new ServiceException("شماره همراه وارد شده تکراری می باشد");

            var newCustomer = new User(command.PhoneNumber, command.FirstName, command.LastName, PersonType.Individual, null, command.City, command.Province)
            {
                Id = command.Id,
                PhoneNumber = command.PhoneNumber,
                NormalizedUserName = null,
                PhoneNumberConfirmed = false,
                Email = null,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
            };
            var createResult = await _userManager.CreateAsync(newCustomer);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newCustomer, RoleType.Customer.ToString());
                await SendCustomerInformation(newCustomer);
                await _bus.Publish(new UserRegisteredEvent()
                {
                    UserId = newCustomer.Id,
                    Name = newCustomer.FirstName + "|" + newCustomer.LastName,
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
                throw new ServiceException(createResult.Errors?.FirstOrDefault()?.Description);
            }
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

        public async Task HandleAsync(ChangeCustomerPhoneNumberCommand command)
        {
            var user = await GetUser(command.UserId);

            var isPhoneNumberExist = _userManager.Users.Any(i => i.PhoneNumber == command.PhoneNumber);

            if (isPhoneNumberExist)
                throw new ServiceException("شماره همراه قبلا استفاده شده است");

            user.PhoneNumber = command.PhoneNumber;
            user.PhoneNumberConfirmed = false;

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

            var nameChanged = user.FirstName != command.FirstName || user.LastName != command.LastName;
            var emailChanged = user.Email != command.Email;

            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.City = command.City;
            user.Province = command.Province;
            user.Email = command.Email;

            var result = await _userManager.UpdateAsync(user);

            await _bus.Publish(new UserProfileUpdatedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = result.Succeeded,
                Name = nameChanged ? command.FirstName + "|" + command.LastName : null,
                PhoneNumber = null,
                TwoFactorEnabled = null,
                Email = emailChanged ? command.Email : null,
            });

            await UpdateCustomerInformation(user, command.PostalCode, command.PostalAddress, command.AccessToken);
        }

        private async Task UpdateCustomerInformation(User user, string postalCode, string postalAddress, string accessToken)
        {
            var _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var updateCustomerCommand = new
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                Province = user.Province,
                PostalCode = postalCode,
                PostalAddress = postalAddress
            };

            var myContent = JsonConvert.SerializeObject(updateCustomerCommand);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using HttpResponseMessage response = await _client.PostAsync(_hostSettings.ShopAddress + $"/Customer/{user.Id}/Update", byteContent);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ReasonPhrase);
                throw new ServiceException("مشکل در برقراری ارتباط");
            }
        }


        public async Task HandleAsync(SendCodeCommand command)
        {
            var user = _userManager.Users.SingleOrDefault(i => i.PhoneNumber == command.PhoneNumber);


            //به خاطر مسائل امنیتی ما به کاربر اعلام نمیکنیم که آیا این یوزر در سیستم وجود دارد یا خیر
            if (user == null)
                return;

            if(user.Status != RegisteryStatus.Accepted)
                throw new ServiceException("ثبت نام شما تایید نشده است");

            // چون هنوز پنل پیامکی نداریم چیزی نمیفرستیم 
            //await _signInService.GenerateAndSendActivationCode(user);            
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
                IsSuccess = true,//isValid,
                CodeType = SmsCodeType.ActivationCode.ToEventEnum(),
                Email = user.Email,
                Mobile = user.PhoneNumber,
                Name = user.FirstName + " " + user.LastName,
            });

            /*if (!isValid)
                throw new ServiceException("کد وارد شده معتبر نمی باشد.");*/

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

            //await _signInService.GenerateAndSendActivationCode(user);

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
