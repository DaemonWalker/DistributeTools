using CSRedis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DistributeTools.DistributeCache.Internal
{
    internal class Cache : ICache
    {
        private readonly IServiceProvider service;
        private readonly CSRedisClient redisClient;
        private readonly DistributeCacheConfig config;
        private ConcurrentDictionary<string, CacheInfo> caches;
        public Cache(IServiceProvider service, DistributeCacheConfig config)
        {
            this.service = service;
            this.config = config;
            this.caches = new ConcurrentDictionary<string, CacheInfo>();
            redisClient = new CSRedisClient(config.RedisConnectionString);
        }
        public byte[] Get(string key)
        {
            if (this.caches.ContainsKey(key))
            {
                return caches[key].GetData();
            }
            return redisClient.Get<byte[]>(key);
        }

        public void Set(string key, byte[] value)
        {
            if (this.caches.TryGetValue(key, out var info))
            {
                info = service.GetService(typeof(CacheInfo)) as CacheInfo;
                info.UpdateData(value);
                this.caches.TryAdd(key, info);
            }
            else
            {
                info.UpdateData(value);
            }
            this.redisClient.SetAsync(key, value, config.ExpireSeconds);
        }
    }
}
