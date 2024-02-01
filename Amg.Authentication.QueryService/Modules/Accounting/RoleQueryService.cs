using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Roles;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Extensions;
using Amg.Authentication.Infrastructure.Helpers;
using Amg.Authentication.QueryModel.Dtos.Accounting;
using Amg.Authentication.QueryModel.Exceptions;
using Amg.Authentication.QueryModel.Services.Accounting;
using Microsoft.AspNetCore.Identity;

namespace Amg.Authentication.QueryService.Modules.Accounting
{
    public class RoleQueryService : IRoleQueryService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleQueryService(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <inheritdoc />
        public async Task<List<RoleInfoDto>> GetRoleInfos()
        {
            // todo :: improve bad implementation
            var roles = RolesParser.ToRoleTypes(_roleManager.Roles.Select(i => i.Name).ToList());
            var result = new List<RoleInfoDto>();
            foreach (var role in roles)
            {
                result.Add(new RoleInfoDto()
                {
                    Name = role.GetDescription(),
                    Type = role.ToString(),
                    TotalUsersInRole = (await _userManager.GetUsersInRoleAsync(role.ToString())).Count
                });
            }

            return result;
        }

        public async Task<RoleInfoDto> GetRoleInfo(string roleName)
        {
            if (!Enum.TryParse(typeof(RoleType), roleName, out var roleType))
                throw new QueryServiceNotFoundException("نقش یافت نشد.");

            var role = (RoleType) roleType;
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
                throw new QueryServiceNotFoundException("نقش یافت نشد.");

            return new RoleInfoDto()
            {
                Name = role.GetDescription(),
                Type = role.ToString(),
                TotalUsersInRole = (await _userManager.GetUsersInRoleAsync(role.ToString())).Count
            };
        }
    }
}
