using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Newtonsoft.Json;

namespace Systemintegration.Messaging
{
    public class Consumer
    {
        private readonly string _queueName;

        public Consumer(string queueName)
        {
            _queueName = queueName;
        }
        public async Task ReceiveMessages()
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

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var deserializedMessage = JsonConvert.DeserializeObject<dynamic>(message);
                    Console.WriteLine(" [x] Received {0}", JsonConvert.SerializeObject(deserializedMessage, Formatting.Indented));
                };
                channel.BasicConsume(queue: _queueName,
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}