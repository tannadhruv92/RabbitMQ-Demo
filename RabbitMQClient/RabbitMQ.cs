using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace RabbitMQClient
{
    public class RabbitMQ : IDisposable
    {
        readonly IConnection connection;
        public object channel;
        public RabbitMQ(string virtualHost = null)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = virtualHost ?? "/";
            factory.HostName = "localhost";            
            connection = factory.CreateConnection();           
        }

        public void Dispose()
        {
            connection.Close();
        }

        /// <summary>
        /// Send messgae to a given queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>  
        public void SendMessage(string queueName, string message)
        {
            if (connection.CloseReason != null || connection == null)
                throw new NullReferenceException("Connection is null");

            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                var _basicProperties = channel.CreateBasicProperties();
                _basicProperties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: _basicProperties,
                                     body: body);
            }
        }

        /// <summary>
        /// Receive message from a given queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="receiveHandler"></param>
        /// <returns></returns>
        public string ReciveMessage(string queueName, object channel,  Action<string, ulong> receiveHandler)
        {
            try
            {
                if (connection.CloseReason != null || connection == null)
                    throw new NullReferenceException("Connection is null");

                var _channel = GetChannel(channel);

                //var subscription = new Subscription()

                _channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                _channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    receiveHandler.Invoke(message, ea.DeliveryTag);
                    _channel.BasicAck(ea.DeliveryTag, false);
                };
                return _channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetMessageCount(string queueName, object channel)
        {
            if (connection.CloseReason != null || connection == null)
                throw new NullReferenceException("Connection is null");

            var _channel = (IModel)channel;

            return (int)_channel.MessageCount(queueName);

        }

        public void PurgeQueue(string queueName)
        {
            if (connection.CloseReason != null || connection == null)
                throw new NullReferenceException("Connection is null");

            using (IModel channel = connection.CreateModel())
            {
                channel.QueuePurge(queueName);
            }
        }

        public object GetChannel()
        {
            if (connection.CloseReason != null || connection == null)
                throw new NullReferenceException("Connection is null");

            channel = connection.CreateModel();

            return channel;
        }

        public int GetConsumerCount(string queueName, object channel)
        {
            if (connection.CloseReason != null || connection == null)
                throw new NullReferenceException("Connection is null");

            IModel _channel = GetChannel(channel);

            return (int)_channel.ConsumerCount(queueName);
        }

        private IModel GetChannel(object channel)
        {
            try
            {
                if (connection.CloseReason != null || connection == null)
                    throw new NullReferenceException("Connection is null");

                if (channel == null)
                {
                    return connection.CreateModel();
                }

                IModel _channel = (IModel)channel;

                if (_channel.IsClosed || _channel.CloseReason != null)
                {
                    _channel = connection.CreateModel();
                }

                return _channel;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void CancelConsuming(string consumerTag, object channel)
        {
            try
            {
                if (connection.CloseReason != null || connection == null)
                    throw new NullReferenceException("Connection is null");

                IModel _channel = GetChannel(channel);

                _channel.BasicCancel(consumerTag);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CloseChannel(object channel)
        {
            IModel _channel = GetChannel(channel);

            _channel.Close();
        }

        public void SendMessageWithTopic(string queueName, string message)
        {
            if (connection.CloseReason != null || connection == null)
                throw new NullReferenceException("Connection is null");

            using (var channel = connection.CreateModel())
            {

                channel.ExchangeDeclare(exchange: "dhruv_message", type: "topic");

                //channel.QueueDeclare(queue: queueName,
                //                     durable: false,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "dhruv_message",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
            }
        }

        public void ReceiveMessageWithTopic(string queueName, Action<string> receiveHandler)
        {
            if (connection.CloseReason != null || connection == null)
                throw new NullReferenceException("Connection is null");

            var _channel = connection.CreateModel();

            string queue = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: queue,
                                  exchange: "dhruv_message",
                                  routingKey: queueName);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                receiveHandler.Invoke(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(queue: queue,
                                 autoAck: false,
                                 consumer: consumer);
        }

        public void OnConnectionBlocked(Action<object, object> connectionBlockedHandler)
        {
            connection.ConnectionBlocked += connectionBlockedHandler.Invoke;
        }

        public void OnConnectionShutDown(Action<object, object> connectionShutDownHandler)
        {
            connection.ConnectionShutdown += connectionShutDownHandler.Invoke;
        }

        public void AckMessage(object channel,  ulong deliveryTag)
        {
            IModel _channel = GetChannel(channel);
            _channel.BasicAck(deliveryTag, false);
        }

    }
}