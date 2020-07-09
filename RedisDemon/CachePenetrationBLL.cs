using RedisBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RedisDemon
{
    public class CachePenetrationBLL
    {
        public string GetDbData()
        {
            Console.WriteLine("数据库查询");
            return null;
        }

        public void SetCache(string value)
        {
            Register.RedisDb.Set("Cache1", value, 1);

        }



        public string GetCache()
        {
            var val =  Register.RedisDb.Get("Cache1");
            if (val == null)
            {
                var dt = GetDbData();
                SetCache(dt);
                return dt;
            }
            return val;
        }


        public string GetCache2()
        {
            var val = Register.RedisDb.Get("Cache1");
            if (val == null)
            {
                var dt = GetDbData();
                if (dt == null)
                {
                    dt = "防止击穿";
                }
                SetCache(dt);
                return dt;
            }
            return val;
        }


    }
}
