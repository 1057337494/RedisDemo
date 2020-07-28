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
            //启动Redis
            RedisDb = new CSRedis.CSRedisClient("localhost:8888,defaultDatabase=09");
        }

        /// <summary>
        /// 字符串输出
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        public async static void WriteColorLine(string str, ConsoleColor color)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = currentForeColor;
        }

        /// <summary>
        /// 数据库数据
        /// </summary>
        private static List<Person> _dbData = new List<Person> {
        new Person(1,"测试001",18),
        new Person(2,"测试002",18),
        new Person(3,"测试003",18),
        new Person(4,"测试004",18),
        new Person(5,"测试005",18),
        new Person(6,"测试006",18),
        };

        /// <summary>
        /// 仿数据库根据ID获取指定数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Person GetDbData(int id)
        {
            WriteColorLine("数据库查询", ConsoleColor.Red);
            return _dbData.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 仿数据库查询所有数据
        /// </summary>
        /// <returns></returns>
        public static List<Person> GetDbAll()
        {
            return _dbData;
        }

    }
}
