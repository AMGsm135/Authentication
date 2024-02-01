using System;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Dtos.Accounting;
using Gridify;

namespace Amg.Authentication.QueryModel.Services.Accounting
{
    public interface IUserActivityQueryService : IQueryService
    {
        /// <summary>
        /// دریافت لیست فعالیت های کاربر توسط خود کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<Paging<UserActivityDto>> GetUserActivitiesByUser(Guid userId, GridifyQuery query);

        /// <summary>
        /// دریافت لیست فعالیت های کاربر توسط مدیر سیستم
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<Paging<UserActivityDto>> GetUserActivitiesByAdmin(Guid userId, GridifyQuery query);
    }
}
