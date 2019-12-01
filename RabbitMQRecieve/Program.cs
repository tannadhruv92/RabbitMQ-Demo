using RabbitMQClient;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RabbitMQRecieve
{
    class Program
    {
        static string consumerTag = string.Empty;
        static RabbitMQ rabbitMQ = null;

        static void Main(string[] args)
        {
            rabbitMQ = new RabbitMQ();
            //rabbitMQ.channel = rabbitMQ.GetChannel();
            rabbitMQ.StartExcusiveConsuming(Guid.NewGuid().ToString("N"));
        }
       
        private static void OnMessageRecived(string obj, ulong deliveryTag)
        {
            Console.WriteLine(obj);
            System.Threading.Tasks.Task.Delay(150000).Wait();
        }

        
    }
}
