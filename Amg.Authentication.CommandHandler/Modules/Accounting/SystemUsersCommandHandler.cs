using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Exceptions;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Events.UserActivities;
using Amg.Authentication.Command.Accounting.FundUsers;
using Amg.Authentication.CommandHandler.Mappers;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Extensions;
using Amg.Authentication.Infrastructure.Helpers;
using Amg.Authentication.Infrastructure.Settings;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Amg.Authentication.CommandHandler.Modules.Accounting
{
    public class SystemUsersCommandHandler : ICommandHandler,
        ICommandHandler<RegisterSystemUserCommand>,
        ICommandHandler<UpdateSystemUserCommand>,
        ICommandHandler<AssignRoleToFundUserCommand>,
        ICommandHandler<UnAssignRoleFromFundUserCommand>,
        ICommandHandler<ActivateUserCommand>,
        ICommandHandler<DeactivateUserCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthSettings _authSettings;
        private readonly IClientInfoGrabber _clientInfoGrabber;
        private readonly IBusControl _bus;

        public SystemUsersCommandHandler(IOptions<AuthSettings> authSettings, UserManager<User> userManager,
            IClientInfoGrabber clientInfoGrabber, IBusControl bus)
        {
            _userManager = userManager;
            _clientInfoGrabber = clientInfoGrabber;
            _bus = bus;
            _authSettings = authSettings.Value;
        }

        public async Task HandleAsync(RegisterSystemUserCommand command)
        {
            var currentUser = await _userManager.FindByNameAsync(command.UserName);
            if (currentUser != null)
                throw new ServiceException("نام کاربری وارد شده تکراری می باشد");

            var newUser = new User(command.UserName, command.FirstName, command.LastName, PersonType.Individual, null, null, null)
            {
                Id = command.Id,
                PhoneNumber = command.PhoneNumber,
                NormalizedUserName = command.UserName.ToUpper(),
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = command.TwoFactorEnabled,
            };

            var createResult = await _userManager.CreateAsync(newUser, command.Password);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, RoleType.SystemUser.ToString());

                await _bus.Publish(new UserRegisteredEvent()
                {
                    UserId = newUser.Id,
                    Name = newUser.FirstName + "|" + newUser.LastName,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    PersonType = newUser.PersonType.ToEventEnum(),
                    ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                    IsSuccess = true,
                    ByAdmin = false,
                });
            }
            else
                throw new ServiceException(createResult.Errors?.FirstOrDefault()?.Description);
        }

        public async Task HandleAsync(UpdateSystemUserCommand command)
        {
            var user = await GetUser(command.UserId);

            var currentUser = await _userManager.FindByNameAsync(command.UserName);
            if (currentUser != null)
                throw new ServiceException("نام کاربری وارد شده تکراری می باشد");

            var activationChanged = user.IsActive != command.IsActive;
            var nameChanged = user.FirstName != command.FirstName || user.LastName != command.LastName;
            var twoFactorChanged = user.TwoFactorEnabled != command.TwoFactorEnabled;

            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.UserName = command.UserName;
            user.NormalizedUserName = command.UserName.ToUpper();
            user.TwoFactorEnabled = command.TwoFactorEnabled;
            user.IsActive = command.IsActive;

            var result = await _userManager.UpdateAsync(user);

            await _bus.Publish(new UserProfileUpdatedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = result.Succeeded,
                Name = nameChanged ? user.FirstName + " " + user.LastName : null,
                PhoneNumber = null,
                TwoFactorEnabled = twoFactorChanged ? new bool?(user.TwoFactorEnabled) : null,
                Email = null,
            });

            if (activationChanged)
            {
                await _bus.Publish(new UserStatusChangedEvent()
                {
                    UserId = user.Id,
                    ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                    IsSuccess = true,
                    IsActive = user.IsActive,
                });
            }
        }

        public async Task HandleAsync(AssignRoleToFundUserCommand command)
        {
            var user = await GetUser(command.UserId);

            var userRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (removeResult.Succeeded)
            {
                var isSuccess = true;
                if (command.Roles?.Any() ?? false)
                    isSuccess = (await _userManager.AddToRolesAsync(user, command.Roles.Select(i => i.ToString()))).Succeeded;

                await _bus.Publish(new UserRolesChangedEvent()
                {
                    UserId = user.Id,
                    ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                    IsSuccess = isSuccess,
                    Role = string.Join(',', command.Roles?
                        .Select(i => i.GetDescription()).ToArray() ?? Array.Empty<string>()),
                });
            }
        }

        public async Task HandleAsync(UnAssignRoleFromFundUserCommand command)
        {
            var user = await GetUser(command.UserId);

            var userRoles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));
            if (userRoles.Contains(command.Role))
            {
                var selectedRole = userRoles.First(i => command.Role == i);
                var result = await _userManager.RemoveFromRoleAsync(user, selectedRole.ToString());

                await _bus.Publish(new UserRolesChangedEvent()
                {
                    UserId = user.Id,
                    ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                    IsSuccess = result.Succeeded,
                    Role = string.Join(',', userRoles.Except(new[] { selectedRole })
                        .Select(i => i.GetDescription()).ToArray()),
                });

            }
        }

        public async Task HandleAsync(ActivateUserCommand command)
        {
            var user = await GetUser(command.UserId);
            if (user.IsActive)
                throw new ServiceException("کاربر هم اکنون فعال می باشد");

            user.Activate();
            await _userManager.UpdateAsync(user);

            await _bus.Publish(new UserStatusChangedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = true,
                IsActive = true,
            });

        }

        public async Task HandleAsync(DeactivateUserCommand command)
        {
            var user = await GetUser(command.UserId);

            if (!user.IsActive)
                throw new ServiceException("کاربر هم اکنون غیر فعال می باشد");

            var roles = RolesParser.ToRoleTypes(await _userManager.GetRolesAsync(user));
            if (roles.Contains(RoleType.SuperAdmin))
                throw new ServiceException("امکان غیر فعال کردن این کاربر وجود ندارد.");

            user.DeActivate();
            await _userManager.UpdateAsync(user);

            await _bus.Publish(new UserStatusChangedEvent()
            {
                UserId = user.Id,
                ClientInfo = _clientInfoGrabber.GetClientInfo().ToEvent(),
                IsSuccess = true,
                IsActive = false,
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
