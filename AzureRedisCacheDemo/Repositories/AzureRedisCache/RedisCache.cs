using Newtonsoft.Json;
using StackExchange.Redis;

namespace AzureRedisCacheDemo.Repositories.AzureRedisCache
{
    public class RedisCache : IRedisCache
    {
        private readonly IDatabase _db;
        public RedisCache(IConfiguration configuration)
        {
            _db = ConnectionMultiplexer.Connect(configuration["RedisURL"]).GetDatabase();
        }

        public T GetCacheData<T>(string key)
        {
            var value = _db.StringGet(key);
            return !string.IsNullOrEmpty(value) ? JsonConvert.DeserializeObject<T>(value) : default;
        }

        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            return _isKeyExist == true ? _db.KeyDelete(key) : (object)false;
        }

        public bool SetCacheData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }
    }
}
