using RabbitMQ.Client;
using System;
using System.Text;

namespace Systemintegration.Messaging
{
    public class Producer
    {
        private readonly string _queueName;
        public Producer(string queueName)
        {
            _queueName = queueName;
        }
        
        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: _queueName,
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}