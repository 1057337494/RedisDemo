using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static RedisBase.Register;

namespace RedisDemon
{
    /// <summary>
    /// 缓存预热
    /// </summary>
    public class CachePreheatBLL
    {
        public CachePreheatBLL()
        {
            //ReloadCache()
        }

        private const string _cacheKey = "PreheatKey";

        private void ReloadCache()
        {
            Thread.Sleep(3 * 1000);
            RedisDb.Set(_cacheKey, "测试预热缓存缓存", 20);
        }


        private string GetCache()
        {
            if (!RedisDb.Exists(_cacheKey))
            {
                ReloadCache();
            }
            return RedisDb.Get(_cacheKey);
        }

        public void GetPreheatCache()
        {
            var inTime = DateTime.Now;
            WriteColorLine($"[{inTime}]进入获取缓存", ConsoleColor.Red);
            WriteColorLine(GetCache(), ConsoleColor.Yellow);
            var outTime = DateTime.Now;
            WriteColorLine($"[{outTime}]结束获取缓存", ConsoleColor.Red);
            WriteColorLine($"用时:{(outTime - inTime).Seconds}秒", ConsoleColor.Red);
        }


    }
}
