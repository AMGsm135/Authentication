using System.Threading;
using System.Threading.Tasks;

namespace Amg.Authentication.Infrastructure.Base
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}
