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

        string cacheKey => $"LowerCacheKey1";
        string cacheKey2 => "LowerCacheKeyBackup";
        public CacheLowerBLL()
        {
            RedisDb.Set(cacheKey, "缓存获取的数据");
            RedisDb.Set(cacheKey2, "低级备份缓存");
        }

        /// <summary>
        /// 启用本地缓存
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 启用二级缓存
        /// </summary>
        /// <returns></returns>
        private string GetCache2()
        {
            string cacheKey2 = "LowerCacheKeyBackup";
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
                return RedisDb.Get(cacheKey2);
            }
        }


        private int _exceptionCount = 0;
        /// <summary>
        /// 禁止访问业务
        /// </summary>
        /// <returns></returns>
        private string GetCache3()
        {
            if(_exceptionCount>10)
            {
                return "业务繁忙，请稍后访问";
            }

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
                _exceptionCount++;
                return "业务繁忙，请稍后访问";
            }
        }


        public void TestLower()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                //WriteColorLine(GetCache(), ConsoleColor.Green);
                WriteColorLine(GetCache2(), ConsoleColor.Green);
                //WriteColorLine(GetCache3(), ConsoleColor.Green);
            }
        }



    }
}
