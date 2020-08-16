using CSRedis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DistributeTools.DistributeCache.Internal
{
    internal class Cache : ICache
    {
        private readonly IServiceProvider service;
        private readonly CSRedisClient redisClient;
        private readonly DistributeCacheConfig config;
        private readonly ConcurrentDictionary<string, CacheInfo> caches;
        private readonly CacheSync sync;
        private ILogger<Cache> logger;
        public Cache(ILogger<Cache> logger, ILogger<CacheSync> syncLogger, IServiceProvider service, DistributeCacheConfig config)
        {
            this.service = service;
            this.config = config;
            this.logger = logger;
            this.caches = new ConcurrentDictionary<string, CacheInfo>();
            redisClient = new CSRedisClient(config.RedisConnectionString);
            this.sync = new CacheSync(this, config);
        }
        public byte[] Get(string key)
        {
            if (this.caches.ContainsKey(key))
            {
                logger.LogInformation($"Found key: '{key}' at local");
                return caches[key].GetData();
            }
            logger.LogInformation($"Fetch key: '{key}' from Redis");
            return redisClient.Get<byte[]>(key);
        }

        public void Set(string key, byte[] value)
        {
            logger.LogInformation($"Set key: '{key}' at local cache");
            if (this.caches.TryGetValue(key, out var info))
            {
                info.UpdateData(value);
            }
            else
            {
                info = service.GetService(typeof(CacheInfo)) as CacheInfo;
                info.UpdateData(value);
                this.caches.TryAdd(key, info);
            }
            logger.LogInformation($"Set key: '{key}' to Redis");
            this.redisClient.PublishAsync(config.SyncChannel, ConvertToSyncMessageJson(this.config.MachineName, key, value));
            this.redisClient.SetAsync(key, value, config.ExpireSeconds);
        }
        internal void InternalUpdate(string key, byte[] value)
        {
            if (caches.TryGetValue(key, out var info))
            {
                logger.LogInformation($"Receive new value from Redis, key: {key}");
                info.UpdateDataWithoutRenew(value);
            }
        }

        private static string ConvertToSyncMessageJson(string machineName, string key, byte[] value)
        {
            var syncInfo = new SyncMessage(machineName, key, value);
            return JsonSerializer.Serialize(syncInfo);
        }
    }
}
