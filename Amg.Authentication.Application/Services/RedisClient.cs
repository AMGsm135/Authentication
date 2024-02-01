using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Amg.Authentication.Application.Services
{

    // a.ammari:
    // به دلایل پرفورمنسی باید کانکشن ردیس به صورت وابستگی مجزای
    // Singleton
    // ثبت گردد.
    public class RedisClient
    {
        public RedisClient(IConfiguration configuration)
        {
            Instance = ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("RedisConnection"));

            Server = Instance.GetServer(Instance.GetEndPoints()[0]);
        }

        public IServer Server { get; }

        public ConnectionMultiplexer Instance { get; }

        public IDatabase Database(int dbNumber) => Instance.GetDatabase(dbNumber);
    }
}
