using RabbitMQClient;
using RabbitMQDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQSend
{
    class Program
    {
        static List<string> messageList = new List<string>();
        static CancellationTokenSource source;
        static RabbitMQ mQ;
        static int counter;
        static int queueCounter = 1;
        private static object lockObject = new object();
        static void Main(string[] args)
        {
            mQ = new RabbitMQ();

            mQ.ConsumeEvents(OnQueueEventRecevied);
        }

        private static void OnQueueEventRecevied(string obj)
        {
            lock (lockObject)
            {
                Console.WriteLine($"{queueCounter++} Queue Deleted : {obj}"); 
            }
        }

        private static void BackUpDemo()
        {
            List<Name> virtualHostList = new RabbitMQAPI().GetVirtualHostList().Where(s => s.messages_ready > 0).ToList();

            if (virtualHostList.Count > 0)
            {
                for (int i = 0; i < virtualHostList.Count; i++)
                {
                    Name name = virtualHostList[i];
                    source = new CancellationTokenSource(); 
                    ReceiveMessageFromVHost(name);
                    Task.Delay(-1, source.Token).ContinueWith(task => { }).Wait();
                }
            }
            Console.WriteLine("Fetched each messages");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void ReceiveMessageFromVHost(Name name)
        {
            //mQ?.Dispose();
            //counter = 0;
            //queueCounter = name.messages_ready;
            //mQ = new RabbitMQ(name.vhost);
            //object channel = mQ.GetChannel();
            //mQ.ReciveMessage(name.name, channel, onMessageReceived);
        }

        private static void onMessageReceived(string arg1, ulong arg2)
        {
            #region Message send
            messageList.Add(arg1);
            #endregion

            counter++;

            if (counter == queueCounter)
            {
                #region Reset counter
                source.Cancel();
                #endregion
            }
        }

        //private static void SendMessage()
        //{
        //    RabbitMQ mQ = new RabbitMQ("MTL278");

        //    for (int i = 0; i < 2; i++)
        //    {
        //        mQ.SendMessage("Tanna", "Hi There");
        //    }

        //    Console.WriteLine("Done");
        //    Console.ReadKey();
        //    Environment.Exit(0);
        //}
    }
}
