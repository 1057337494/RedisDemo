﻿using RedisBase;
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
        /// <summary>
        /// 全表重建缓存导致无法缓存不存在的Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Person GetCache2(int id)
        {
            var cacheKey = "PenetrationCache2";
            var getCache = RedisDb.HGet(cacheKey, id.ToString());
            if (getCache == null)
            {
                //全表重建缓存
                var pinple = RedisDb.StartPipe();
                foreach (var item in GetDbAll())
                {
                    pinple.HSet(cacheKey, item.Id.ToString(), item);
                }
                pinple.EndPipe();
                getCache = RedisDb.HGet(cacheKey, id.ToString());
            }
            RedisDb.Expire(cacheKey, 10);
            if (getCache == null)
            {
                return GetDbData(id);
            }
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(getCache, new Person());
        }

        /// <summary>
        /// 单个缓存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Person GetCache(int id)
        {
            var cacheKey = "PenetrationCache2";
            var getCache = RedisDb.HGet(cacheKey, id.ToString());
            if (getCache == null)
            {
                var dt = GetDbData(id);
                RedisDb.HSet(cacheKey, id.ToString(), dt);
                return dt;
            }
            RedisDb.Expire(cacheKey, 10);
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(getCache, new Person());
        }


        public void TestPenetration()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1);
                var id = new Random().Next(1, 15);
                WriteColorLine($"查询ID:[{id}]", ConsoleColor.Gray);
                //GetCache2(id);
                GetCache(id);
            }
        }


      


    }
}
