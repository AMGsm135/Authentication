using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Application.Contract.Services;
using StackExchange.Redis;
using ClientInfo = Amg.Authentication.Application.Contract.Dtos.ClientInfo;

namespace Amg.Authentication.Application.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IDatabase _database;
        private readonly RedisClient _redisClient;
        private const int DatabaseNumber = 2;

        public TokenManager(RedisClient redisClient)
        {
            _redisClient = redisClient;
            _database = _redisClient.Database(DatabaseNumber);
        }

        /// <inheritdoc />
        public async Task AddToken(UserTokenItem item, TimeSpan expire)
        {
            var (key, value) = SerializeUserToken(item);
            await _database.StringSetAsync(key, value, expire);
        }

        /// <inheritdoc />
        public async Task<UserTokenItem> GetToken(Guid tokenId, Guid userId)
        {
            var key = GetKeyPattern(tokenId, userId);
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;
            var result = DeserializeUserToken(key, value);
            return result;
        }

        /// <inheritdoc />
        public async Task<UserTokenItem> GetTokenById(Guid tokenId)
        {
            var pattern = GetKeyPattern(tokenId, null);
            var key = (await _redisClient.Server.KeysAsync(DatabaseNumber, pattern).ToListAsync()).FirstOrDefault();
            if (key.Equals(new RedisKey())) // is null
                return null;
            var value = await _database.StringGetAsync(key);
            var result = DeserializeUserToken(key, value);
            return result;
        }

        /// <inheritdoc />
        public async Task<List<UserTokenItem>> GetTokensByUserId(Guid userId)
        {
            var pattern = GetKeyPattern(null, userId);
            var keys = await _redisClient.Server.KeysAsync(DatabaseNumber, pattern).ToListAsync();

            var result = new List<UserTokenItem>();
            foreach (var key in keys)
            {
                var value = _database.StringGet(key);
                var item = DeserializeUserToken(key, value);
                result.Add(item);
            }
            return result;
        }

        /// <inheritdoc />
        public async Task RemoveToken(Guid tokenId, Guid userId)
        {
            var pattern = GetKeyPattern(tokenId, userId);
            await _database.KeyDeleteAsync(pattern);
        }

        /// <inheritdoc />
        public async Task RemoveToken(Guid tokenId)
        {
            var pattern = GetKeyPattern(tokenId, null);
            var key = (await _redisClient.Server.KeysAsync(DatabaseNumber, pattern).ToListAsync()).FirstOrDefault();
            if (!key.Equals(new RedisKey())) // is null
            {
                await _database.KeyDeleteAsync(key);
            }
        }




        #region Private Methods

        private string GetKeyPattern(Guid? tokenId, Guid? userId)
        {
            return (tokenId == null ? "*" : tokenId.Value.ToString("N")) +
                   "-" + (userId == null ? "*" : userId.Value.ToString("N"));
        }

        private string GetKey(Guid tokenId, Guid userId)
        {
            return GetKeyPattern(tokenId, userId);
        }

        private (Guid TokenId, Guid UserId) ParseKey(string key)
        {
            var parts = key.Split("-");
            return (Guid.Parse(parts[0]), Guid.Parse(parts[1]));
        }

        private (string Key, string Value) SerializeUserToken(UserTokenItem item)
        {
            var key = GetKey(item.TokenId, item.UserId);
            var value = JsonSerializer.Serialize(item.ClientInfo);
            return (key, value);
        }

        private UserTokenItem DeserializeUserToken(string key, string value)
        {
            var id = ParseKey(key);
            var clientInfo = JsonSerializer.Deserialize<ClientInfo>(value);
            return new UserTokenItem()
            {
                TokenId = id.TokenId,
                UserId = id.UserId,
                ClientInfo = clientInfo,
            };
        }

        #endregion

    }

}
