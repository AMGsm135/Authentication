using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Groups.Interfaces;
using Amg.Authentication.DomainModel.Modules.Permissions.Interfaces;
using Amg.Authentication.QueryModel.Dtos.Authorization;
using Amg.Authentication.QueryModel.Exceptions;
using Amg.Authentication.QueryModel.Services.Authorization;
using Amg.Authentication.QueryService.Extensions;
using Gridify;

namespace Amg.Authentication.QueryService.Modules.Authorization
{
    public class GroupQueryService : IGroupQueryService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GroupQueryService(IGroupRepository groupRepository, IPermissionRepository permissionRepository)
        {
            _groupRepository = groupRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<Paging<GroupDto>> GetByFilterAsync(GridifyQuery query)
        {
            // todo :: fix bad implementation
            var groups = await _groupRepository.GetByFilterAsync(query);
            return new Paging<GroupDto>(groups.Count,
                groups.Data.Select(i => i.ToDto(_groupRepository.TotalUsersCountAsync(i.Id).Result)));
        }

        public async Task<GroupDto> GetByIdAsync(Guid groupId)
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new QueryServiceNotFoundException("گروه کاربری یافت نشد.");

            var usersCount = await _groupRepository.TotalUsersCountAsync(groupId);
            return group.ToDto(usersCount);
        }

        public async Task<IList<GroupPermissionDto>> GetGroupPermissionsAsync(Guid groupId)
        {
            var groupPermissions = await _groupRepository.GetGroupPermissionsAsync(groupId);
            var permissions = await _permissionRepository.GetAllAsync();

            var permissionsDto = permissions.GroupBy(p => p.Category)
                .Select(i => new GroupPermissionDto
                {
                    Category = i.Key,
                    Permissions = i.Select(permission => new GroupPermissionItemDto
                    {
                        PermissionId = permission.Id,
                        PermissionName = permission.Name,
                        PermissionDescription = permission.Description,
                        HasPermission = groupPermissions.Any(groupPermission => groupPermission.Permission.Id == permission.Id)
                    }).ToList()
                }).ToList();

            return permissionsDto;
        }

        public async Task<IList<string>> GetGroupPermissionNamesAsync(params Guid[] groupIds)
        {
            var groupPermissions = await _groupRepository.GetGroupPermissionsAsync(groupIds);
            var names = groupPermissions.Select(i => i.Permission.Name).ToList();
            return names;
        }

        public async Task<IList<string>> GetUserPermissionNamesAsync(Guid userId, string serviceName)
        {
            var userGroups = await _groupRepository.GetAllUserGroupsAsync(userId);
            var userGroupIds = userGroups.Select(i => i.Group.Id).ToArray();
            var userGroupPermissions = await _groupRepository.GetGroupPermissionsAsync(serviceName, userGroupIds);
            var permissionNames = userGroupPermissions.Select(groupPermission => groupPermission.Permission.Name).ToList();
            return permissionNames;
        }

        public async Task<IList<UserGroupDto>> GetUserGroupsAsync(Guid userId)
        {
            var userGroups = await _groupRepository.GetAllUserGroupsAsync(userId);
            var groups = await _groupRepository.GetAllAsync();

            var userGroupDtos = groups.Select(i => new UserGroupDto
            {
                GroupId = i.Id,
                GroupName = i.Name,
                IsInGroup = userGroups.Any(q => q.Group.Id == i.Id)
            }).ToList();

            return userGroupDtos;
        }

        public async Task<IList<string>> GetUserGroupNamesAsync(Guid userId)
        {
            var userGroups = await _groupRepository.GetAllUserGroupsAsync(userId);
            return userGroups.Select(i => i.Group.Name).ToList();
        }

        public async Task<bool> HasGroupPermissionAsync(string permissionName, string serviceName, params Guid[] groupIds)
        {
            var result = await _groupRepository.HasGroupPermissionAsync(permissionName, serviceName, groupIds);
            return result;
        }

        public async Task<bool> HasUserPermissionAsync(string permissionName, string serviceName, Guid userId)
        {
            var result = await _groupRepository.HasUserPermissionAsync(permissionName, serviceName, userId);
            return result;
        }

        public async Task<UserGroupPermissionsDto> GetUserGroupPermissionsAsync(Guid userId)
        {
            var userGroups = await _groupRepository.GetAllUserGroupsAsync(userId);
            var userGroupIds = userGroups.Select(i => i.Group.Id).ToArray();
            var userGroupPermissions = await _groupRepository.GetGroupPermissionsAsync(userGroupIds);
            var permissionNames = userGroupPermissions.Select(groupPermission => groupPermission.Permission.Name).ToList();

            var userGroupPermissionsDto = new UserGroupPermissionsDto
            {
                FirstGroupName = userGroups.Any() ? userGroups.First().Group.Name : null,
                PermissionsNames = permissionNames
            };

            return userGroupPermissionsDto;
        }
    }
}
