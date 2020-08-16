using System;
using System.Collections.Generic;
using System.Text;

namespace DistributeTools.DistributeCache
{
    public class DistributeCacheConfig
    {
        public int ExpireSeconds { get; set; }
        public string RedisConnectionString { get; set; }
        public string SyncChannel { get; set; }
        public string MachineName { get; set; } = Guid.NewGuid().ToString("N");
    }
}
