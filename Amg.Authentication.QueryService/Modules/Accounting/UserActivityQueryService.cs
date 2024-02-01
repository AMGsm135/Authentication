using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.UserActivities.Interfaces;
using Amg.Authentication.QueryModel.Dtos.Accounting;
using Amg.Authentication.QueryModel.Services.Accounting;
using Amg.Authentication.QueryService.Extensions;
using Gridify;

namespace Amg.Authentication.QueryService.Modules.Accounting
{
    public class UserActivityQueryService : IUserActivityQueryService
    {
        private readonly IUserActivityRepository _userActivityRepository;

        public UserActivityQueryService(IUserActivityRepository userActivityRepository)
        {
            _userActivityRepository = userActivityRepository;
        }

        /// <inheritdoc />
        public async Task<Paging<UserActivityDto>> GetUserActivitiesByUser(Guid userId, GridifyQuery query)
        {
            var activities = await _userActivityRepository.GetByFilterForUserAsync(userId, query);
            var result = new Paging<UserActivityDto>(activities.Count, activities.Data.Select(i => i.ToDto()));
            return result;
        }

        /// <inheritdoc />
        public async Task<Paging<UserActivityDto>> GetUserActivitiesByAdmin(Guid userId, GridifyQuery query)
        {
            var activities = await _userActivityRepository.GetByFilterForAdminAsync(userId, query);
            var result = new Paging<UserActivityDto>(activities.Count, activities.Data.Select(i => i.ToDto()));
            return result;
        }
    }
}
