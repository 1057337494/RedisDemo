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
            //Penetration();

            //new RedisTypeBLL().ShowType();

            // new DoubleBuffBLL().TestDoubleBuff();

            new CachePenetrationBLL().Penetration();

            Console.WriteLine("Hello World!");
        }

 


       






    }
}
