using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Groups;
using Amg.Authentication.DomainModel.Modules.Groups.Entities;
using Amg.Authentication.DomainModel.Modules.Groups.Interfaces;
using Amg.Authentication.Persistence.Contexts;
using Amg.Authentication.Persistence.Repositories.Base;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Amg.Authentication.Persistence.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(DatabaseContext context) : base(context)
        {
        }

        #region Group

        /// <inheritdoc />
        public async Task<List<Group>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Paging<Group>> GetByFilterAsync(GridifyQuery query)
        {
            return await DbSet.GridifyAsync(query);
        }

        /// <inheritdoc />
        public async Task<List<Group>> GetByIdsAsync(params Guid[] ids)
        {
            return await DbSet.Where(i => ids.Contains(i.Id)).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Group> GetByIdAsync(Guid id)
        {
            return await DbSet.FirstOrDefaultAsync(i => i.Id == id);
        }

        #endregion


        #region GroupPermission

        /// <inheritdoc />
        public async Task<Group> GetIncludePermissionsByIdAsync(Guid groupId)
        {
            return await DbSet.Include(i => i.GroupPermissions)
                .FirstOrDefaultAsync(i => i.Id == groupId);
        }

        /// <inheritdoc />
        public async Task<bool> HasGroupPermissionAsync(string permissionName, string serviceName, params Guid[] groupIds)
        {
            return await DbSet.AnyAsync(g =>
                groupIds.Contains(g.Id) &&
                g.GroupPermissions.Any(p =>
                    p.Permission.Name == permissionName &&
                    p.Permission.ServiceName == serviceName));
        }

        /// <inheritdoc />
        public async Task<List<GroupPermission>> GetGroupPermissionsAsync(params Guid[] groupIds)
        {
            return await DbSet.Where(g => groupIds.Contains(g.Id))
                .SelectMany(i => i.GroupPermissions)
                .Include(i => i.Permission)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<GroupPermission>> GetGroupPermissionsAsync(string serviceName, params Guid[] groupIds)
        {
            return await DbSet.Where(g => groupIds.Contains(g.Id))
                .SelectMany(i => i.GroupPermissions)
                .Include(i => i.Permission)
                .Where(i => i.Permission.ServiceName == serviceName)
                .ToListAsync();
        }

        #endregion


        #region GroupUser

        /// <inheritdoc />
        public async Task<int> TotalUsersCountAsync(Guid groupId)
        {
            return await DbSet.Where(g => g.Id == groupId)
                .Select(i => i.GroupUsers.Count)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<List<GroupUser>> GetAllUserGroupsAsync(Guid userId)
        {
            return await DbSet.SelectMany(i => i.GroupUsers)
                .Include(i => i.Group)
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public Task<bool> HasUserPermissionAsync(string permissionName, string serviceName, Guid userId)
        {
            return DbSet.AnyAsync(g =>
                g.GroupUsers.Any(u => u.UserId == userId) &&
                g.GroupPermissions.Any(p =>
                    p.Permission.Name == permissionName &&
                    p.Permission.ServiceName == serviceName));
        }

        #endregion

    }

}
