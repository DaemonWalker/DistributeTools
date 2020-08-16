using CSRedis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static CSRedis.CSRedisClient;

namespace DistributeTools.DistributeCache.Internal
{
    internal class CacheSync
    {
        private readonly CSRedisClient redisClient;
        private readonly Encoding encoding = Encoding.UTF8;
        private readonly DistributeCacheConfig config;
        private readonly Cache cache;
        public CacheSync(Cache cache, DistributeCacheConfig config)
        {
            this.config = config;
            this.cache = cache;
            redisClient = new CSRedisClient(config.RedisConnectionString);
            if (string.IsNullOrEmpty(config.SyncChannel))
            {
                redisClient.Subscribe((
                    nameof(config.SyncChannel),
                    this.Sync));
            }
        }
        private void Sync(SubscribeMessageEventArgs args)
        {
            SyncMessage msg = args.Deserialize();
            if (msg.MachineName != config.MachineName)
            {
                cache.InternalUpdate(msg.Key, msg.Value);
            }
        }
    }
    internal class SyncMessage
    {
        public string MachineName { get; set; }
        public string Key { get; set; }
        public byte[] Value { get; set; }
    }
}
