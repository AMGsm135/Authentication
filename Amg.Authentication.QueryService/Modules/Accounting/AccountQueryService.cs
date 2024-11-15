using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Groups;
using Amg.Authentication.DomainModel.Modules.Groups.Interfaces;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Extensions;
using Amg.Authentication.QueryModel.Dtos.Accounting;
using Amg.Authentication.QueryModel.Exceptions;
using Amg.Authentication.QueryModel.Services.Accounting;
using Amg.Authentication.QueryService.Extensions;
using Gridify;
using Microsoft.AspNetCore.Identity;

namespace Amg.Authentication.QueryService.Modules.Accounting
{
    public class AccountQueryService : IAccountQueryService
    {
        private readonly UserManager<User> _userManager;
        private readonly IGroupRepository _groupRepository;

        public AccountQueryService(UserManager<User> userManager, IGroupRepository groupRepository)
        {
            _userManager = userManager;
            _groupRepository = groupRepository;
        }

        /// <inheritdoc />
        public async Task<bool> IsUserExists(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            var user = await _userManager.FindByNameAsync(userName);
            return user != null;
        }

        /// <inheritdoc />
        public async Task<UserDto> GetUserById(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new QueryServiceNotFoundException("کاربر یافت نشد.");

            var userGroups = await _groupRepository.GetAllUserGroupsAsync(userId);
            var group = userGroups.Select(i => i.Group).ToList();

            var roles = await _userManager.GetRolesAsync(user);
            return user.ToDto(roles, group);

        }

        public async Task<Paging<UserDto>> GetFundUsersByFilterAsync(GridifyQuery query)
        {
            // todo :: improve bad implementation
            var fundUsers = await _userManager.GetUsersInRoleAsync(RoleType.SystemUser.ToString());
            var filteredUsers = fundUsers.AsQueryable().GridifyQueryable(query);
            var fundUserIds = fundUsers.Select(i => i.Id).ToList();

            var dictionary = new Dictionary<Guid, List<Group>>();

            foreach (var fundUser in filteredUsers.Query)
            {
                var userGroups = await _groupRepository.GetAllUserGroupsAsync(fundUser.Id);
                dictionary.Add(fundUser.Id, userGroups.Select(i => i.Group).ToList());
            }


            var groups = await _groupRepository.GetAllAsync();

            return new Paging<UserDto>(filteredUsers.Count, filteredUsers.Query
                    .Select(i => i.ToDto(_userManager.GetRolesAsync(i).Result, dictionary[i.Id])));
        }

        public async Task<List<UserRoleDto>> GetUserRoles(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new QueryServiceNotFoundException("کاربر یافت نشد.");

            var userRoles = Enum.GetValues(typeof(RoleType))
                .Cast<RoleType>()
                .Select(i => new UserRoleDto()
                {
                    RoleType = i.ToString(),
                    RoleName = i.GetDescription(),
                    IsInRole = _userManager.IsInRoleAsync(user, i.ToString()).Result
                })
                .ToList();

            return userRoles;
        }
    }
}
