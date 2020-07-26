using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RedisBase.Register;

namespace RedisDemon
{
    public class RedisPuReBLl
    {

        private const string _cancelName = "cancel";

        public RedisPuReBLl()
        {
            Task.Run(() => {
                int i = 0;
                while (true)
                {
                  
                    Publish(ref i);
                }
            
            });

            Task.Run(() => {

                for (int i = 0; i < 3; i++)
                {
                    Received(i);
                }
                  
                

            });


            Thread.Sleep(100*1000);
        }

        private void Publish(ref int i)
        {
           
            RedisDb.Publish(_cancelName, $"[{DateTime.Now.ToString("hh:MM:ss.fffffff")}][{i++}]发布消息");
            Thread.Sleep(new Random().Next(0,2000));
        }

        private void Received(int i)
        {
            RedisDb.Subscribe((_cancelName, msg => { WriteColorLine($"[{i}][{DateTime.Now.ToString("hh:MM:ss.fffffff")}]接收消息:[{msg.Body}]", (ConsoleColor) i);  }));
        }


    }
}
