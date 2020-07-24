using CSRedis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;

namespace RedisDemon
{
    public class RedisLockBLL
    {
        private void DoSomething(int id)
        {
            var cacheKey = $"RedisLock:{id}";

            using (GetRedisLock(cacheKey))
            {
                WriteColorLine($"正在处理ID【{id}】", ConsoleColor.Green);
                Thread.Sleep(1 * 1000);
                WriteColorLine($"处理ID结束【{id}】", ConsoleColor.Red);
            }
        }


        private void DoSomething2(int id)
        {
            var cacheKey = $"RedisLock2:{id}";

            

            if (RedisDb.SetNx(cacheKey, "1"))
            {
                RedisDb.Expire(cacheKey, 5);
                WriteColorLine($"正在处理ID【{id}】", ConsoleColor.Green);
                Thread.Sleep(1 * 1000);
                WriteColorLine($"处理ID结束【{id}】", ConsoleColor.Red);
                RedisDb.Del(cacheKey);
            }
            else
            {
                WriteColorLine($"ID【{id}】正在处理中", ConsoleColor.Yellow);
            }
        }


        private CSRedisClientLock GetRedisLock(string cacheKey)
        {
            using (var mx = new Mutex(true, cacheKey))
            {
                mx.WaitOne();
                var getLock = RedisDb.Lock(cacheKey, 2);
                mx.Close();
                return getLock;
            };
        }


        public void TestLock()
        {
           
            for (int i = 0; i < 100; i++)
            {
               
                Thread.Sleep(100);
                Task.Run(() =>
                {
                    var count = i;
                    var id = count % 6;
                    //DoSomething(id);
                    DoSomething2(id);

                    WriteColorLine($"第[{count}]次执行成功", ConsoleColor.White);
                   
                });
                
            }

           
            Thread.Sleep(30*1000);
           
        }




    }
}
