﻿using DistributeTools.DistributeCache.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DistributeTools.DistributeCache
{
    public static class DependencyInjection
    {
        public static void AddDistributeCache(this IServiceCollection services, Action<DistributeCacheConfig> configAction)
        {
            var config = new DistributeCacheConfig();
            config.MachineName = Guid.NewGuid().ToString("N");
            config.SyncChannel = nameof(config.SyncChannel);
            configAction.Invoke(config);
            services.AddSingleton<ICache, Cache>();
            services.AddTransient<CacheInfo>();
            services.AddSingleton(config);
        }
    }
}
