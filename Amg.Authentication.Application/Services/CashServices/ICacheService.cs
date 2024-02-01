using System;
using System.Threading.Tasks;

namespace Amg.Authentication.Application.Services.CashServices
{
    public interface ICacheService
    {
        Task<T> GetData<T>(string key);

        /// <returns></returns>
        bool SetData<T>(string key, T value, TimeSpan expirationTime);

        object RemoveData(string key);

        bool ExistsInCache(string key);

        TimeSpan GetTimeToLive(string key);

        DateTime GetExpireTime(string key);
    }
}
