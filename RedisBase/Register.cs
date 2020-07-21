using CSRedis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static List<Person> _dbData = new List<Person> {
        new Person(1,"测试001",18),
        new Person(2,"测试002",18),
        new Person(3,"测试003",18),
        new Person(4,"测试004",18),
        new Person(5,"测试005",18),
        new Person(6,"测试006",18),
        };

        public static Person GetDbData(int id)
        {
            WriteColorLine("数据库查询", ConsoleColor.Red);
            return _dbData.Where(s => s.Id == id).FirstOrDefault();
        }

        public static List<Person> GetDbAll()
        {
            return _dbData;
        }

    }
}
