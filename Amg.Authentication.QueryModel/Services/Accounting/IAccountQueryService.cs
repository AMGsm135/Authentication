using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Dtos.Accounting;
using Gridify;

namespace Amg.Authentication.QueryModel.Services.Accounting
{
    public interface IAccountQueryService : IQueryService
    {
        /// <summary>
        /// آیا کاربر با نام کاربری وارد شده موجود است؟
        /// </summary>
        /// <param name="userName">نام کاربری</param>
        /// <returns></returns>
        Task<bool> IsUserExists(string userName);
        
        /// <summary>
        /// دریافت اطلاعات کاربر توسط شناسه
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserDto> GetUserById(Guid userId);
        
        /// <summary>
        /// دریافت اطلاعات کاربران صندوق به شکل فیلتر شده، صفحه بندی شده و مرتب سازی شده
        /// </summary>
        /// <param name="query">درخواست فیلتر، صفحه بندی و مرتب سازی</param>
        /// <returns></returns>
        Task<Paging<UserDto>> GetFundUsersByFilterAsync(GridifyQuery query);

        /// <summary>
        /// دریافت لیست نقش های یک کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserRoleDto>> GetUserRoles(Guid userId);
    }
}
