using System;
using RabbitMQ.Client;

namespace TimeToBeReceived
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string queueName = "FlightInfoQueue";

                // Declare the queue
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                // Initialize Producer
                var producer = new Producer(channel, queueName);

                // Send multiple messages
                var messages = new[]
                {
                    "SAS3732,2023-10-01T10:00:00",
                    "KLM1456,2023-10-01T12:00:00",
                    "SW4789,2023-10-01T14:00:00"
                };

                foreach (var message in messages)
                {
                    producer.SendMessage(message);
                }

                // Initialize Consumer
                var consumer = new Adapter.Consumer(channel, queueName);
                consumer.ReceiveMessage();

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}