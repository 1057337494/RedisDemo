using Newtonsoft.Json;
using RedisBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static RedisBase.Register;

namespace RedisDemon
{
    /// <summary>
    /// 多级缓存
    /// </summary>
    public class CacheLevelBLL
    {

        private string _cacheKey => "LevelCache";

        private void SetChace()
        {


            //全表重建缓存
            var pinple = RedisDb.StartPipe();
            foreach (var item in GetDbAll())
            {
                pinple.HSet(_cacheKey, item.Id.ToString(), item);
            }
            pinple.EndPipe();
        }


        private static Dictionary<int, Person> _localCache = new Dictionary<int, Person>();

        public CacheLevelBLL()
        {
            SetChace();
        }

        private Person GetCache(int id)
        {
            if (_localCache.ContainsKey(id))
            {
                return _localCache[id];
            }

            var getH = RedisDb.HGet(_cacheKey, id.ToString());
            if (!string.IsNullOrWhiteSpace(getH))
            {
                var model = JsonConvert.DeserializeAnonymousType(getH, new Person());
                //_localCache.Add(id, model);
                return model;
            }
            return GetDbData(id);
        }


        public void TestCacheLevel()
        {
            var count = GetDbAll().Count;
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(10);
                var model = GetCache(i % count + 1);
                WriteColorLine(JsonConvert.SerializeObject(model), ConsoleColor.Yellow);
            }




        }




    }
}
