using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.UserActivities;
using Amg.Authentication.DomainModel.Modules.UserActivities.Interfaces;
using Amg.Authentication.Persistence.Contexts;
using Amg.Authentication.Persistence.Repositories.Base;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Amg.Authentication.Persistence.Repositories
{
    public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<Paging<UserActivity>> GetByFilterForAdminAsync(Guid userId, GridifyQuery query)
        {
            return await GetIncludeEntities()
                .Where(i => i.UserId == userId)
                .GridifyAsync(query);
        }

        public async Task<Paging<UserActivity>> GetByFilterForUserAsync(Guid userId, GridifyQuery query)
        {
            return await GetIncludeEntities()
                .Where(i => i.UserId == userId && !i.Activity.IsAdministrative)
                .GridifyAsync(query);
        }

        private IQueryable<UserActivity> GetIncludeEntities()
        {
            return DbSet
                .Include(i => i.ClientInfo)
                .Include(i => i.Activity);
        }
    }
}
