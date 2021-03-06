﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;

namespace RedisDemon
{
    /// <summary>
    /// 双缓冲模式
    /// </summary>
    public class DoubleBufferBLL
    {

        public DoubleBufferBLL()
        {
            RedisDb.Set(_cacheList[_reloadCache], $"[{DateTime.Now}]{_cacheList[_reloadCache]}");
            RedisDb.Set(_cacheList[_usingCache], $"[{DateTime.Now}]{_cacheList[_usingCache]}");

        }

        #region 双缓冲
        /// <summary>
        /// 控制双缓冲Key
        /// </summary>
        private List<string> _cacheList => new List<string> {
        "DoubleBuffCache1",
        "DoubleBuffCache2",
        };

        /// <summary>
        /// 标识当前使用的缓存
        /// </summary>
        private int _usingCache = 0;

        /// <summary>
        /// 标识当前执行更新的缓存
        /// </summary>
        private int _reloadCache => _usingCache == 0 ? 1 : 0;

        /// <summary>
        /// 标识状态
        /// </summary>
        private bool _isReloading = false;

        /// <summary>
        /// 标识下次刷新时间
        /// </summary>
        private DateTime _nextReloadTime = DateTime.Now;

        /// <summary>
        /// 切换缓存
        /// </summary>
        private void SwapCache()
        {
            _usingCache = _usingCache == 0 ? 1 : 0;
            WriteColorLine($"缓存切换{_cacheList[_reloadCache]}=>{_cacheList[_usingCache]}", ConsoleColor.Green);
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        private void ReloadCache()
        {
            _isReloading = true;
            WriteColorLine($"正在执行缓存刷新", ConsoleColor.Green);
            Thread.Sleep(2 * 1000);

            RedisDb.Set(_cacheList[_reloadCache], $"[{DateTime.Now}]{_cacheList[_reloadCache]}");
            RedisDb.Expire(_cacheList[_reloadCache], 30);

            _nextReloadTime = DateTime.Now.AddSeconds(1);
            WriteColorLine($"执行缓存刷新完毕", ConsoleColor.Green);
            _isReloading = false;
        }
        #endregion



        private string ReadCache()
        {
            if (DateTime.Now > _nextReloadTime)
            {
                lock (this)
                {
                    if (!_isReloading)
                    {
                        SwapCache();
                        Task.Run(() =>
                        {
                            ReloadCache();
                        });
                    }
                }
            }

            return RedisDb.Get(_cacheList[_usingCache]);
        }


        /// <summary>
        /// 阻塞方式获取缓存
        /// </summary>
        /// <returns></returns>
        private string ReadCacheByLock()
        {
            if (DateTime.Now > _nextReloadTime)
            {
                lock (this)
                {
                    if (DateTime.Now > _nextReloadTime)
                    {
                        ReloadCache();
                    }
                }
            }
            return RedisDb.Get(_cacheList[_reloadCache]);
        }

        private bool _isLock = false;
        /// <summary>
        /// 部分阻塞获取缓存
        /// </summary>
        /// <returns></returns>
        private string ReadCacheByLockExpire()
        {
            if (DateTime.Now > _nextReloadTime && !_isLock)
            {
                _isLock = true;
                lock (this)
                {
                    if (DateTime.Now > _nextReloadTime)
                    {
                        ReloadCache();
                    }
                }
                _isLock = false;
            }
            return RedisDb.Get(_cacheList[_reloadCache]);
        }





        public void TestDoubleBuff()
        {
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(10);

                //Task.Run(() => WriteColorLine($"[{DateTime.Now}]:::::::" + ReadCache(), ConsoleColor.Yellow));
                //Task.Run(() => WriteColorLine($"[{DateTime.Now}]:::::::" + ReadCacheByLock(), ConsoleColor.Yellow));
                Task.Run(() => WriteColorLine($"[{DateTime.Now}]:::::::" + ReadCacheByLockExpire(), ConsoleColor.Yellow));
            }
        }






    }
}
