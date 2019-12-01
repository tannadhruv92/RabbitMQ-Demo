using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQClient 
{
    public class RabbitMQEvents : DefaultBasicConsumer
    {
        public delegate void HandleQueueEvent(string queueName);
        public event HandleQueueEvent QeueuEvent;

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            string queueName = Encoding.UTF8.GetString((byte[])properties.Headers["name"]);
            QeueuEvent(queueName);
            base.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body); 
        }

        public void ConsumeRabbitMQEvents(IModel channel, string queue)
        {
            channel.BasicConsume(queue, true, this);
        }
    }
}
