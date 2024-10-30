using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TransactionalClient.RabbitMQ
{
    public class Consumer
    {
        public static void ReceiveMessages()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "passengerQueue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueDeclare(queue: "luggageQueue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var passengerConsumer = new EventingBasicConsumer(channel);
                passengerConsumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Received passenger info: {message}");
                    try
                    {
                        // Process message
                        channel.BasicAck(ea.DeliveryTag, false); // Acknowledge message
                    }
                    catch (Exception)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, true); // Negative acknowledge in case of error
                    }
                };
                channel.BasicConsume(queue: "passengerQueue",
                    autoAck: false, // Disable auto-acknowledgment
                    consumer: passengerConsumer);

                var luggageConsumer = new EventingBasicConsumer(channel);
                luggageConsumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Received luggage info: {message}");
                    try
                    {
                        // Process message
                        channel.BasicAck(ea.DeliveryTag, false); // Acknowledge message
                    }
                    catch (Exception)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, true); // Negative acknowledge in case of error
                    }
                };
                channel.BasicConsume(queue: "luggageQueue",
                    autoAck: false, // Disable auto-acknowledgment
                    consumer: luggageConsumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}