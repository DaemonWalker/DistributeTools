using CSRedis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
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
            if (string.IsNullOrEmpty(config.SyncChannel) == false)
            {
                redisClient.Subscribe((
                    config.SyncChannel,
                    this.Sync));
            }
        }
        private void Sync(SubscribeMessageEventArgs args)
        {
            var msg = Deserialize(args.Body);
            if (msg.MachineName != this.config.MachineName)
            {
                cache.InternalUpdate(msg.Key, msg.GetBytes());
            }
        }

        private static SyncMessage Deserialize(string body)
        {
            return JsonSerializer.Deserialize<SyncMessage>(body);
        }
    }
    internal class SyncMessage
    {
        public SyncMessage() { }
        public SyncMessage(string machineName, string key, byte[] value)
        {
            this.MachineName = machineName;
            this.Key = key;
            this.Value = Convert.ToBase64String(value);
        }
        public string MachineName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public byte[] GetBytes()
        {
            return Convert.FromBase64String(Value);
        }
    }
}
