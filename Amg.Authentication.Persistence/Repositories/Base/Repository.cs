using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Amg.Authentication.Persistence.Repositories.Base
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(DatabaseContext context)
        {
            DbSet = context.Set<TEntity>();
        }

        public ValueTask<TEntity> FindAsync(params object[] keys)
        {
            return DbSet.FindAsync(keys);
        }

        public Task AddAsync(TEntity entity)
        {
            DbSet.AddAsync(entity);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.AddRangeAsync(entities);
            return Task.CompletedTask;
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    }
}
