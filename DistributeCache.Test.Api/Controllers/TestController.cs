using DistributeTools.DistributeCache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributeCache.Test.Api.Controllers
{
    [ApiController]
    [Route("api/cache")]
    public class TestController : ControllerBase
    {
        private readonly ICache cache;
        public TestController(ICache cache)
        {
            this.cache = cache;
        }
        [HttpGet]
        public string Get(string key)
        {
            return cache.GetString(key);
        }

        [HttpPost]
        public IActionResult Set([FromBody] CacheInfo cacheInfo)
        {
            cache.SetString(cacheInfo.Key, cacheInfo.Value);
            return Ok();
        }
    }
    public class CacheInfo
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
