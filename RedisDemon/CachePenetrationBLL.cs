using RedisBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;

namespace RedisDemon
{
    /// <summary>
    /// 缓存雪崩
    /// </summary>
    public class CachePenetrationBLL
    {
        public CachePenetrationBLL()
        {
            new Task(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await SetCache(i);
                }
            });
        }

        private string GetDbData(int i)
        {
            var data = $"数据[{i}]";
            WriteColorLine($"数据库查询数据[{i}]", ConsoleColor.Green);
            return data;
        }

        private async Task SetCache(int i)
        {
            Thread.Sleep(1 * 1000);
            await Register.RedisDb.SetAsync($"Cache{i}", GetDbData(i), 2);
        }

        private async Task SetCacheWithRadom()
        {
            await RedisDb.SetAsync("Cache1", "缓存数据", 1 + new Random().Next());


        }

        private async Task<string> GetCache(int i)
        {
            var val = await Register.RedisDb.GetAsync($"Cache{i}");
            if (val == null)
            {
               
                

                await SetCache(i);
                return GetDbData(i);
            }
            return val;

        }

        public void Penetration()
        {
            //1.同一时间大量缓存失效
            for (int i = 0; i < 10; i++)
            {
               
                WriteColorLine(GetCache(i), ConsoleColor.Red);

            }



            //2.热点数据失效

            //void Show()
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        var str = bll.GetCache().Result;
            //        if (str.Contains("数据库"))
            //        {
            //            WriteColorLine(str + $"[{DateTime.Now}]", ConsoleColor.Red);
            //        }
            //        else
            //        {
            //            //WriteColorLine(str, ConsoleColor.White);
            //        }
            //    }
            //}

            //var taskArr = new Task[100];
            //for (int i = 0; i < 100; i++)
            //{
            //    taskArr[i] = (new Task(Show));
            //    taskArr[i].Start();
            //}

            //Task.WaitAll(taskArr);
        }




    }
}
