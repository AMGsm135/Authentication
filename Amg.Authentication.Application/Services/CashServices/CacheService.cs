using Amg.Authentication.Infrastructure.Settings;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Amg.Authentication.Application.Services.CashServices
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cashDataBase;
        private const int DataBaseNumber = 3;

        public CacheService(RedisClient redisClient)
        {
            _cashDataBase = redisClient.Database(DataBaseNumber);
        }

        public async Task<T> GetData<T>(string key)
        {
            var value = _cashDataBase.StringGet(key);

            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }
        public bool SetData<T>(string key, T value, TimeSpan expirationTime)
        {
            var isSet = _cashDataBase.StringSet(key, JsonConvert.SerializeObject(value), expirationTime);
            return isSet;
        }
        public object RemoveData(string key)
        {
            bool _isKeyExist = _cashDataBase.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _cashDataBase.KeyDelete(key);
            }
            return false;
        }

        public bool ExistsInCache(string key)
        {
            return _cashDataBase.KeyExists(key);
        }

        public TimeSpan GetTimeToLive(string key)
        {
            return (TimeSpan)_cashDataBase.KeyTimeToLive(key);
        }

        public DateTime GetExpireTime(string key)
        {
            return (DateTime)_cashDataBase.KeyExpireTime(key);
        }
    }
}
