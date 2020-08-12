using System;
using System.Collections.Generic;
using System.Text;

namespace DistributeTools.DistributeCache
{
    public interface ICache
    {
        void Set(string key, byte[] value);
        byte[] Get(string key);
    }
}
