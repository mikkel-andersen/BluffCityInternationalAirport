using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Alternativ_Til_Publisher_Subscriber
{
    public class Consumer
    {
        private readonly string _queueName;
        private readonly IConnection _connection;
        private readonly MessageFilter _messageFilter;

        public Consumer(string queueName, IConnection connection, string airline)
        {
            _queueName = queueName;
            _connection = connection;
            _messageFilter = new MessageFilter(airline);
        }

        public async Task ReceiveMessages()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"Received message: {message}");

                    if (_messageFilter.FilterMessage(message))
                    {
                        var etaMessage = JsonSerializer.Deserialize<ETA>(message);
                        Console.WriteLine($"Processed message: {message}");
                    }
                    else
                    {
                        Console.WriteLine($"Message filtered out: {message}");
                    }
                };

                channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

                // Keep the consumer running
                await Task.Delay(-1);
            }
        }
    }
}