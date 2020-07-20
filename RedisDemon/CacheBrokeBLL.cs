using RedisBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RedisDemon
{
    /// <summary>
    /// 缓存穿透
    /// </summary>
    public class CacheBrokeBLL
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


        //static void Penetration()
        //{
        //    var bll = new CachePenetrationBLL();

        //    void Show()
        //    {
        //        for (int i = 0; i < 1000; i++)
        //        {
        //            var str = bll.GetCache();
        //            WriteColorLine(str + $"[{DateTime.Now}]", ConsoleColor.Red);
        //        }

        //    }

        //    Show();
        //}
        //static void Penetration2()
        //{
        //    var bll = new CachePenetrationBLL();

        //    void Show()
        //    {
        //        for (int i = 0; i < 1000; i++)
        //        {
        //            var str = bll.GetCache2();
        //            if (str.Contains("数据库"))
        //            {
        //                WriteColorLine(str + $"[{DateTime.Now}]", ConsoleColor.Red);
        //            }
        //            else
        //            {
        //                WriteColorLine(str, ConsoleColor.White);
        //            }
        //        }
        //    }

        //    Show();
        //}

       



    }
}
