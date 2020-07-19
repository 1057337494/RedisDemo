using RedisBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;
namespace RedisDemon
{
    /// <summary>
    /// 缓存穿透
    /// </summary>
    public class CachePenetrationBLL
    {
        public class Person
        {
            public Person()
            {
            }

            public Person(int id, string name, int age)
            {
                Id = id;
                Name = name;
                Age = age;
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private List<Person> _dbData = new List<Person> { 
        new Person(1,"测试001",18),
        new Person(2,"测试002",18),
        new Person(3,"测试003",18),
        new Person(4,"测试004",18),
        new Person(5,"测试005",18),
        new Person(6,"测试006",18),  
        };


        private Person GetDbData(int id)
        {
            WriteColorLine ("数据库查询",ConsoleColor.Gray);
            return _dbData.Where(s => s.Id == id).FirstOrDefault();
        }

        //public void SetCache(string value)
        //{
        //    Register.RedisDb.Set("Cache1", value, 1);

        //}



        private Person GetCache(int id)
        {
            var cacheKey = "PenetrationCache1";

            var getCache = RedisDb.HGet(cacheKey, id.ToString());
            if (getCache == null)
            {
                var dt = GetDbData(id);
                Register.RedisDb.HSet(cacheKey, id.ToString(), dt);
                return dt;
            }
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(getCache,new Person());
        }


        public void TestPenetration()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1);
                GetCache(new Random().Next(1, 15));
            }



        }


        //public string GetCache2()
        //{
        //    var val = Register.RedisDb.Get("Cache1");
        //    if (val == null)
        //    {
        //        var dt = GetDbData();
        //        if (dt == null)
        //        {
        //            dt = "防止击穿";
        //        }
        //        SetCache(dt);
        //        return dt;
        //    }
        //    return val;
        //}


    }
}
