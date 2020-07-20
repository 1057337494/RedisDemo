using CSRedis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisBase
{
    public static class Register
    {
        public static CSRedisClient RedisDb { get; set; }

         static Register()
        {
            RedisDb = new CSRedis.CSRedisClient("192.168.0.223:6379,defaultDatabase=09");
        }

        public async static void WriteColorLine(string str, ConsoleColor color)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = currentForeColor;
        }

    }
}
