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
    /// 缓存血崩
    /// </summary>
    public class CacheBrokeBLL
    {
        public CacheBrokeBLL()
        {
            //预热缓存

            //var allData = GetDbAll();
            //foreach (var item in allData)
            //{
            //    SetCache(item);
            //    SetCache2(item);
            //    SetCache3(item);
            //}
            //Thread.Sleep(1 * 1000);
        }

        #region 同时失效

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void SetCache(Person model, bool isRandom)
        {
            lock (model)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(1 * 1000);
                    var cacheKey = $"Broke:{model.Id}";

                    if (isRandom)
                    {
                        RedisDb.Set(cacheKey, model, 2 + new Random().Next(0, 20) / 10);
                    }
                    else
                    {
                        RedisDb.Set(cacheKey, model, 2);
                    }
                });
            }
        }



        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cacheInt"></param>
        /// <returns></returns>
        private Person GetCache(int id,bool isRandom, ref int cacheInt)
        {
            var cacheKey = $"Broke:{id}";

            var val = Register.RedisDb.Get(cacheKey);
            if (val == null)
            {
                var model = GetDbData(id);
                SetCache(model, isRandom);
                return model;
            }
            WriteColorLine("缓存获取数据成功", ConsoleColor.Green);
            cacheInt++;
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(val, new Person());
        }

        /// <summary>
        /// 测试 随机时间缓存雪崩
        /// </summary>
        /// <param name="isRandom"></param>
        public void TestBroke(bool isRandom)
        {
            var alldata = GetDbAll();
            foreach (var item in alldata)
            {
                SetCache(item,isRandom);
            }
            Thread.Sleep(1 * 1000);

            int cacheInt = 0;
            var allCount = GetDbAll().Count;
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(100);
                var id = i % allCount + 1;
                WriteColorLine($"查询ID:[{id}]", ConsoleColor.Gray);
                GetCache(id, isRandom, ref cacheInt);
            }
        }

        #endregion



        private static object _lock2 = new object();
        private void SetCache2(Person model)
        {
            lock (_lock2)
            {
                var allData = GetDbAll();
                Task.Run(() =>
                {
                    var cacheKey = "BrokeBigKey";
                    Thread.Sleep(1 * 1000);
                    //全表重建缓存                 

                    RedisDb.HSet(cacheKey, model.Id.ToString(), model);
                    RedisDb.Expire(cacheKey, 1);

                });
            }
        }

        private Person GetCache2(int id)
        {

            var cacheKey = "BrokeBigKey";
            var val = Register.RedisDb.HGet(cacheKey, id.ToString());
            if (val == null)
            {
                var model = GetDbData(id);
                SetCache2(model);
                return model;
            }

            WriteColorLine("缓存获取数据成功", ConsoleColor.Green);
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(val, new Person());
        }


        public void BrokeTest2()
        {
            var allCount = GetDbAll().Count;
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(10);
                var id = i % allCount + 1;
                WriteColorLine($"查询ID:[{id}]", ConsoleColor.Gray);
                GetCache2(id);
            }
        }



        private static object _lock3 = new object();
        private void SetCache3(Person model)
        {
            lock (_lock3)
            {

                Task.Run(() =>
                {

                    Thread.Sleep(1 * 1000);
                    //全表重建缓存  
                    var cacheKey = $"BrokeBigKey:{SimpleHash(model.Id)}";
                    RedisDb.HSet(cacheKey, model.Id.ToString(), model);
                    RedisDb.Expire(cacheKey, 1 + new Random().Next(0, 20) / 10);
                });
            }
        }

        private Person GetCache3(int id)
        {

            var cacheKey = $"BrokeBigKey:{SimpleHash(id)}";
            var val = Register.RedisDb.HGet(cacheKey, id.ToString());
            if (val == null)
            {
                var model = GetDbData(id);
                SetCache3(model);
                return model;
            }

            WriteColorLine("缓存获取数据成功", ConsoleColor.Green);
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(val, new Person());
        }
        private int SimpleHash(int id)
        {
            return id % 4;
        }


        public void BrokeTest3()
        {
            var allCount = GetDbAll().Count;
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(10);
                var id = i % allCount + 1;
                WriteColorLine($"查询ID:[{id}]", ConsoleColor.Gray);
                GetCache3(id);
            }
        }

    }
}
