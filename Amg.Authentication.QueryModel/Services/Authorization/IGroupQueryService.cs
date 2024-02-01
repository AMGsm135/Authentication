using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Dtos.Authorization;
using Gridify;

namespace Amg.Authentication.QueryModel.Services.Authorization
{
    public interface IGroupQueryService : IQueryService
    {
        public Task<Paging<GroupDto>> GetByFilterAsync(GridifyQuery query);
        
        Task<GroupDto> GetByIdAsync(Guid groupId);
        
        Task<IList<GroupPermissionDto>> GetGroupPermissionsAsync(Guid groupId);
        
        Task<IList<string>> GetGroupPermissionNamesAsync(params Guid[] groupIds);

        Task<IList<string>> GetUserPermissionNamesAsync(Guid userId, string serviceName);

        Task<IList<UserGroupDto>> GetUserGroupsAsync(Guid userId);

        Task<IList<string>> GetUserGroupNamesAsync(Guid userId);

        Task<bool> HasGroupPermissionAsync(string permissionName, string serviceName, params Guid[] groupIds);

        Task<bool> HasUserPermissionAsync(string permissionName, string serviceName, Guid userId);

        Task<UserGroupPermissionsDto> GetUserGroupPermissionsAsync(Guid userId);
    }
}
