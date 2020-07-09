using RedisBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RedisDemon
{
    public class CacheBrokeBLL
    {
        public string GetDbData()
        {
            Console.WriteLine("数据库查询");
            return null;
        }

        public async Task SetCache()
        {
            await Register.RedisDb.SetAsync("Cache1", "缓存查询", 1);

        }

        public async Task SetCacheWithRadom()
        {
            await Register.RedisDb.SetAsync("Cache1", "缓存数据", 1+new Random().Next());

        }

        public async Task<string> GetCache()
        {
            var val = await Register.RedisDb.GetAsync("Cache1");
            if (val == null)
            {
                await SetCache();
                return GetDbData();
            }
            return val;

        }


        


    }
}
