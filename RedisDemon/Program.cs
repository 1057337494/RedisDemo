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

            new RedisTypeBLL().ShowType();

            Console.WriteLine("Hello World!");
        }

        static void Penetration()
        {
            var bll = new CachePenetrationBLL();

            void Show()
            {
                for (int i = 0; i < 1000; i++)
                {
                    var str = bll.GetCache();
                    WriteColorLine(str + $"[{DateTime.Now}]", ConsoleColor.Red);
                }

            }

            Show();
        }
        static void Penetration2()
        {
            var bll = new CachePenetrationBLL();

            void Show()
            {
                for (int i = 0; i < 1000; i++)
                {
                    var str = bll.GetCache2();
                    if (str.Contains("数据库"))
                    {
                        WriteColorLine(str + $"[{DateTime.Now}]", ConsoleColor.Red);
                    }
                    else
                    {
                        WriteColorLine(str, ConsoleColor.White);
                    }
                }
            }

            Show();
        }


        static void Broke()
        {
            var bll = new CacheBrokeBLL();

            void Show()
            {
                for (int i = 0; i < 1000; i++)
                {
                    var str = bll.GetCache().Result;
                    if (str.Contains("数据库"))
                    {
                        WriteColorLine(str + $"[{DateTime.Now}]", ConsoleColor.Red);
                    }
                    else
                    {
                        //WriteColorLine(str, ConsoleColor.White);
                    }
                }
            }

            var taskArr = new Task[100];
            for (int i = 0; i < 100; i++)
            {
                taskArr[i] = (new Task(Show));
                taskArr[i].Start();
            }

            Task.WaitAll(taskArr);
        }






    }
}
