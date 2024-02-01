using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Gridify;

namespace Amg.Authentication.DomainModel.Modules.Permissions.Interfaces
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        /// <summary>
        /// دریافت لیست همه دسترسی ها
        /// </summary>
        /// <returns></returns>
        Task<List<Permission>> GetAllAsync();

        /// <summary>
        /// دریافت لیست دسترسی ها به شکل فیلتر، صفحه بندی و مرتب سازی شده
        /// </summary>
        /// <param name="query">درخواست فیلتر، صفحه بندی و مرتب سازی</param>
        /// <returns></returns>
        Task<Paging<Permission>> GetByFilterAsync(GridifyQuery query);

        /// <summary>
        /// دریافت لیست دسترسی ها
        /// </summary>
        /// <param name="ids">شناسه دسترسی ها</param>
        /// <returns></returns>
        Task<List<Permission>> GetByIdsAsync(params Guid[] ids);

        /// <summary>
        /// دریافت اطلاعات دسترسی
        /// </summary>
        /// <param name="id">شناسه دسترسی</param>
        /// <returns></returns>
        Task<Permission> GetByIdAsync(Guid id);


    }
}
