using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Opgave1.RabbitMQ
{
    public class Consumer
    {
        public static void ReceiveMessages(string nationality)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: nationality,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Received message for {nationality}: {message}");
                };
                channel.BasicConsume(queue: nationality,
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
            
        }
        public static void StartConsumersForNationalities(IEnumerable<string> nationalities)
        {
            foreach (var nationality in nationalities)
            {
                ReceiveMessages(nationality);
            }
        }
    }
    
}