using RedisBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static RedisBase.Register;

namespace RedisDemon
{
    /// <summary>
    /// 缓存降级
    /// </summary>
    public class CacheLowerBLL
    {
        private static Dictionary<int, string> _localCache = new Dictionary<int, string>() { [1] = "获取缓存失败，启用本地数据" };

        private static object _lock = new object();
        string cacheKey => $"LowerCacheKey1";
        public CacheLowerBLL()
        {
            RedisDb.Set("LowerCacheKey1", "缓存获取的数据");

        }

        private string GetCache()
        {
            try
            {
                if (new Random().Next(0, 10) > 6)
                {
                    throw new Exception();
                }
                return RedisDb.Get(cacheKey);
            }
            catch (Exception ex)
            {
                return _localCache[1];
            }
        }


        public void TestLower()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                WriteColorLine(GetCache(), ConsoleColor.Green);
            }
        }



    }
}
