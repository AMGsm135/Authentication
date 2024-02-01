using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Groups.Entities;
using Amg.Authentication.Infrastructure.Base;
using Gridify;

namespace Amg.Authentication.DomainModel.Modules.Groups.Interfaces
{
    public interface IGroupRepository : IRepository<Group>
    {
        #region Group

        /// <summary>
        /// دریافت لیست همه گروه ها
        /// </summary>
        /// <returns></returns>
        Task<List<Group>> GetAllAsync();

        /// <summary>
        /// دریافت لیست گروه ها به شکل فیلتر، صفحه بندی و مرتب سازی شده
        /// </summary>
        /// <param name="query">درخواست فیلتر، صفحه بندی و مرتب سازی</param>
        /// <returns></returns>
        Task<Paging<Group>> GetByFilterAsync(GridifyQuery query);

        /// <summary>
        /// دریافت لیست گروه ها
        /// </summary>
        /// <param name="ids">شناسه گروه ها</param>
        /// <returns></returns>
        Task<List<Group>> GetByIdsAsync(params Guid[] ids);

        /// <summary>
        /// دریافت اطلاعات گروه
        /// </summary>
        /// <param name="id">شناسه گروه</param>
        /// <returns></returns>
        Task<Group> GetByIdAsync(Guid id);

        #endregion


        #region GroupPermission

        /// <summary>
        /// دریافت اطلاعات گروه به همراه لیست دسترسی ها
        /// </summary>
        /// <param name="groupId">شناسه گروه</param>
        /// <returns></returns>
        Task<Group> GetIncludePermissionsByIdAsync(Guid groupId);

        /// <summary>
        /// آیا گروه دسترسی مشخص شده را دارد ؟
        /// </summary>
        /// <param name="permissionName">نام دسترسی</param>
        /// <param name="serviceName">نام سرویس</param>
        /// <param name="groupIds">شناسه های گروه</param>
        /// <returns></returns>
        Task<bool> HasGroupPermissionAsync(string permissionName, string serviceName, params Guid[] groupIds);

        /// <summary>
        /// دریافت لیست دسترسی های گروه
        /// </summary>
        /// <param name="groupIds">شناسه گروه ها</param>
        /// <returns></returns>
        Task<List<GroupPermission>> GetGroupPermissionsAsync(params Guid[] groupIds);

        /// <summary>
        /// دریافت لیست دسترسی های گروه
        /// </summary>
        /// <param name="groupIds">شناسه گروه ها</param>
        /// <param name="serviceName">نام سرویس</param>
        /// <returns></returns>
        Task<List<GroupPermission>> GetGroupPermissionsAsync(string serviceName, params Guid[] groupIds);

        #endregion


        #region GroupUser

        /// <summary>
        /// دریافت تعداد کاربران یک گروه
        /// </summary>
        /// <param name="groupId">شناسه گروه</param>
        /// <returns></returns>
        Task<int> TotalUsersCountAsync(Guid groupId);

        /// <summary>
        /// دریافت لیست گروه های یک کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<GroupUser>> GetAllUserGroupsAsync(Guid userId);

        /// <summary>
        /// آیا شخص دسترسی مشخص شده را دارد ؟
        /// </summary>
        /// <param name="permissionName">نام دسترسی</param>
        /// <param name="serviceName">نام سرویس</param>
        /// <param name="userId">شناسه شخص</param>
        /// <returns></returns>
        Task<bool> HasUserPermissionAsync(string permissionName, string serviceName, Guid userId);

        #endregion
    }
}
