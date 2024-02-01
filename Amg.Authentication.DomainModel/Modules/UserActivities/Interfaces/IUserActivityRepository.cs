using System;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Gridify;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.Interfaces
{
    public interface IUserActivityRepository : IRepository<UserActivity>
    {
        Task<Paging<UserActivity>> GetByFilterForUserAsync(Guid userId, GridifyQuery query);

        Task<Paging<UserActivity>> GetByFilterForAdminAsync(Guid userId, GridifyQuery query);
    }
}
