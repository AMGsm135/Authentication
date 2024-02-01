using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amg.Authentication.Infrastructure.Base
{
    public interface IRepository
    {
        // base interface for scanning Repositories
    }

    public interface IRepository<TEntity> : IRepository where TEntity : class
    {
        ValueTask<TEntity> FindAsync(params object[] keys);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);
    }
}
