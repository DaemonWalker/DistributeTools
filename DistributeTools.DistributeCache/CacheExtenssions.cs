using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DistributeTools.DistributeCache
{
    public static class CacheExtenssions
    {
        public static void SetInt32(this ICache cache, string key, int value)
        {
            byte[] value2 = new byte[]
            {
                (byte)(value >> 24),
                (byte)(255 & value >> 16),
                (byte)(255 & value >> 8),
                (byte)(255 & value)
            };
            cache.Set(key, value2);
        }
        public static int? GetInt32(this ICache cache, string key)
        {
            byte[] array = cache.Get(key);
            if (array == null || array.Length < 4)
            {
                return null;
            }
            return new int?((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
        }
        public static void SetString(this ICache cache, string key, string value)
        {
            cache.Set(key, Encoding.UTF8.GetBytes(value));
        }

        // Token: 0x0600001A RID: 26 RVA: 0x00003D10 File Offset: 0x00002D10
        public static string GetString(this ICache cache, string key)
        {
            byte[] array = cache.Get(key);
            if (array == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(array);
        }
        public static void SetObj(this ICache cache, string key, object value)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, value);
                    cache.Set(key, ms.GetBuffer());
                }
            }
            catch (Exception e)
            {
                throw new Exception("There's an error throw when serialize the object to binary array.", e);
            }
        }

        public static T GetObject<T>(this ICache cache, string key)
        {
            using (MemoryStream ms = new MemoryStream(cache.Get(key)))
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
