using CSRedis;
using System;
using System.Collections.Generic;
using System.Text;

namespace DistributeTools.DistributeCache.Internal
{
    internal class CacheInfo
    {
        private byte[] data;
        private readonly DistributeCacheConfig config;
        public CacheInfo(DistributeCacheConfig config)
        {
            this.config = config;
            this.ExpireTime = DateTime.Now.AddSeconds(this.config.ExpireSeconds);
        }
        public DateTime ExpireTime { get; set; }
        private void RenewCache()
        {
            this.ExpireTime = DateTime.Now.AddSeconds(this.config.ExpireSeconds);
        }
        internal void UpdateDataWithoutRenew(byte[] value)
        {
            this.data = value;
        }
        internal void UpdateData(byte[] value)
        {
            this.data = value;
            this.RenewCache();
        }
        internal byte[] GetData()
        {
            this.RenewCache();
            return this.data;
        }
    }
}
