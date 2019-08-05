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
            rabbitMQ.OnConnectionBlocked(onConnectionBlcked);
            rabbitMQ.OnConnectionShutDown(onConnectionBlcked);
            rabbitMQ.channel = rabbitMQ.GetChannel();
            rabbitMQ.ReciveMessage("Dhruv", OnMessageRecived);
        }

        private static void onConnectionBlcked(object arg1, object arg2)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(arg2));
        }

        private static void OnMessageRecived(string obj, ulong deliveryTag)
        {
            try
            {
               System.Threading.Tasks.Task.Run(()=>  ShowPopUpAndAckMessage(obj, deliveryTag));
            }
            catch (Exception)
            {

            }
        }

        private static void ShowPopUpAndAckMessage(string obj, ulong deliveryTag)
        {
            MessageBox.Show(obj);
            rabbitMQ.AckMessage(rabbitMQ.channel, deliveryTag);
        }
    }
}
