using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;

namespace RedisDemon
{
    public class DoubleBuffBLL
    {
        private List<string> _cacheList => new List<string> {
        "DoubleBuffCache1",
        "DoubleBuffCache2",
        };

        private int _usingCache = 0;
        private int _reloadCache => _usingCache == 0 ? 1 : 0;

        private bool _isReloading = false;

        private DateTime _nextReloadTime = DateTime.Now;

        public DoubleBuffBLL()
        {
            RedisDb.Set(_cacheList[_reloadCache], $"[{DateTime.Now}]{_cacheList[_reloadCache]}");
            RedisDb.Set(_cacheList[_usingCache], $"[{DateTime.Now}]{_cacheList[_usingCache]}");

        }

        private void SwapCache()
        {
            _usingCache = _usingCache == 0 ? 1 : 0;
            WriteColorLine($"缓存切换{_cacheList[_reloadCache]}=>{_cacheList[_usingCache]}", ConsoleColor.Green);
        }


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


        public void TestDoubleBuff()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                WriteColorLine($"[{DateTime.Now}]:::::::"+ReadCache(), ConsoleColor.Yellow);

            }
        }






    }
}
