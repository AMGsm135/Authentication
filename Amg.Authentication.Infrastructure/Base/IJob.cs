using System.Threading.Tasks;

namespace Amg.Authentication.Infrastructure.Base
{
    public interface IJob
    {
        Task Execute();
    }
    
    public interface IJob<TParam>
    {
        Task Execute(TParam jobParameters);
    }
}
