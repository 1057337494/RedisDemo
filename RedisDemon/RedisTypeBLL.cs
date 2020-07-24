using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;

namespace RedisDemon
{
    public class RedisTypeBLL
    {
        public void ShowType()
        {
            //ShowList();
            //ShowStr();

            //ShowGeo();
            //ShowSet();

            //ShowHash();

            ShowSortSet();
        }

        /// <summary>
        /// 列表
        /// </summary>
        private void ShowList()
        {
            //场景一：使用队列令牌进行熔断限流
            var cacheKey = "ListKey1";
            void SendToken()
            {
                //每秒生成三个令牌 上限为10个令牌
                while (true)
                {
                    Thread.Sleep(1 * 1000);
                    var len = RedisDb.LLen(cacheKey);

                    var insArr = new string[len + 3 > 10 ? 10 - len : 3];
                    for (int i = 0; i < insArr.Length; i++)
                    {
                        insArr[i] = "1";
                    }


                    RedisDb.LPush(cacheKey, insArr);

                }
            }

            Task.Run(() =>
            {
                SendToken();
            });

            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(100);
                if (RedisDb.LPop(cacheKey) == "1")
                {
                    WriteColorLine("成功获取令牌，正在执行业务", ConsoleColor.Red);
                }
                else
                {
                    WriteColorLine("获取令牌失败，访问超时", ConsoleColor.Green);
                }
            }



        }

        /// <summary>
        /// 字符串
        /// </summary>
        private void ShowStr()
        {
            RedisDb.Set("StrKey1", new List<int> { 1, 2, 3, 4, 5 });
            RedisDb.Set("StrKey2", new { Name = "LGn", Age = 18 });
            RedisDb.Set("StrKey3", new[] { new { Name = "LGn", Age = 18 } });

            WriteColorLine(RedisDb.Get("StrKey1"), ConsoleColor.Green);
            WriteColorLine(RedisDb.Get("StrKey2"), ConsoleColor.Green);
            WriteColorLine(RedisDb.Get("StrKey3"), ConsoleColor.Green);


            RedisDb.Set("Incr1", 5);
            RedisDb.Set("Incr2", 5);


            RedisDb.IncrBy("Incr1");

            //异常
            //RedisDb.Set("Incr3", "Abc");
            //RedisDb.IncrBy("Incr3");


        }

        /// <summary>
        /// 集合
        /// </summary>
        private void ShowSet()
        {
            //获取人员访问记录
            var userList = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                userList.Add($"Account{i}");
            }
            var cacheKey = "AccountIn";
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1);
                RedisDb.SAdd(cacheKey, userList[new Random().Next(0, userList.Count)]);
            }
            RedisDb.Expire(cacheKey, 2);
            WriteColorLine("访问人数:" + RedisDb.SCard(cacheKey), ConsoleColor.Blue);
            foreach (var item in RedisDb.SMembers(cacheKey))
            {
                WriteColorLine(item, ConsoleColor.Blue);
            }

        }

        /// <summary>
        /// 有序集合
        /// </summary>
        private void ShowSortSet()
        {
            var cacheKey = "SortZetExpireOrder";
            void CreateOrder()
            {
                var orderId = Guid.NewGuid();
                WriteColorLine($"创建订单ID:【{orderId}】", ConsoleColor.Yellow);
                var orderExpireTimeSpan = DateTime.Now.AddSeconds(3).Ticks;
                RedisDb.ZAdd(cacheKey, (orderExpireTimeSpan, orderId));
            }


            void ExpireOrder()
            {
                while (true)
                {
                    var getKey = RedisDb.ZRangeByScore(cacheKey, 0.ToString(), DateTime.Now.Ticks.ToString(), 1);
                    RedisDb.ZRem(cacheKey, getKey);
                    Thread.Sleep(100);
                    if(getKey.Length>0)
                    {
                        WriteColorLine($"订单ID过期:【{getKey[0]}】", ConsoleColor.Blue);
                    }
                }
            }

            Task.Run(() => ExpireOrder());

           
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                CreateOrder();
            }

            Thread.Sleep(100 * 1000);
        }


        /// <summary>
        /// 地图
        /// </summary>
        private void ShowGeo()
        {
            var ran = new Random();
            for (int i = 0; i < 100; i++)
            {
                RedisDb.GeoAdd("GeoKey1", new Random(Guid.NewGuid().GetHashCode()).Next(10400, 10600) / 100m, new Random(Guid.NewGuid().GetHashCode()).Next(7900, 8100) / 100m, $"mem{i}");
            }

            var myPlace = (105, 80);
            RedisDb.GeoAdd("GeoKey1", myPlace.Item1, myPlace.Item2, $"myplace");

            RedisDb.Expire("GeoKey1", 5);

            var getGeo = RedisDb.GeoRadiusByMemberWithDistAndCoord("GeoKey1", "myplace", 10 * 1000);



            foreach (var item in getGeo)
            {
                WriteColorLine($"Name:{item.member},经度:{item.longitude},纬度:{item.latitude},距离:{item.dist}", ConsoleColor.White);
            }


        }


        internal class AccountInfo
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        /// <summary>
        /// 哈希
        /// </summary>
        private void ShowHash()
        {
            var cacheKey = "HSKey1";
            RedisDb.HSet(cacheKey, Guid.Parse("a590cc18-9bed-46cc-b856-781b27a96798").ToString(), new AccountInfo { Name = "Test1", Age = 18 });
            RedisDb.HSet(cacheKey, Guid.Parse("359bbaec-a900-4a28-8f44-39dfd9fb54fa").ToString(), new AccountInfo { Name = "Test2", Age = 20 });
            RedisDb.HSet(cacheKey, Guid.Parse("7e4645de-059f-4c10-be06-6a8751032d71").ToString(), new AccountInfo { Name = "Test3", Age = 22 });
            RedisDb.HSet(cacheKey, Guid.Parse("a590cc18-9bed-46cc-b856-781b27a96798").ToString(), new AccountInfo { Name = "Test4", Age = 24 });

            WriteColorLine(RedisDb.HGet(cacheKey, Guid.Parse("7e4645de-059f-4c10-be06-6a8751032d71").ToString()), ConsoleColor.Green);


            var cacheKey2 = "HSInrKey1";
            RedisDb.HSet(cacheKey2, 1.ToString(), 10);
            RedisDb.HSet(cacheKey2, 2.ToString(), 10);
            RedisDb.HSet(cacheKey2, 3.ToString(), 10);
            RedisDb.HIncrBy(cacheKey2, 2.ToString());
            RedisDb.HIncrBy(cacheKey2, 5.ToString());
        }

    }
}
