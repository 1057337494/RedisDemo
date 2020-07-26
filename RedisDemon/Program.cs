using RedisBase;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;

namespace RedisDemon
{
    class Program
    {
        static void Main(string[] args)
        {
            //new CachePenetrationBLL().TestPenetration();
            //new CacheBrokeBLL().BrokeTest();
            //new CacheBrokeBLL().BrokeTest2();
            //new CacheBrokeBLL().BrokeTest3();

            //new RedisTypeBLL().ShowType();

            //new DoubleBuffBLL().TestDoubleBuff();

             new CacheLowerBLL().TestLower();

            //new CachePreheatBLL().GetPreheatCache();

            // new RedisTypeBLL().ShowType();

            //new RedisLockBLL().TestLock();


            //new RedisPuReBLl();

            Console.WriteLine("Hello World!");
        }
    }
}
