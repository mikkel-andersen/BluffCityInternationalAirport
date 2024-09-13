using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;

namespace Systemintegration.Messaging
{
    public class Producer
    {
        private readonly string _queueName;
        private readonly IConnection _connection;

        public Producer(string queueName, IConnection connection)
        {
            _queueName = queueName;
            _connection = connection;
        }

        public void SendMessage(object message)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(exchange: "",
                    routingKey: _queueName,
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" [x] Sent {0}", JsonConvert.SerializeObject(message));
            }
        }
    }
}