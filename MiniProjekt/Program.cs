using System;
using RabbitMQ.Client;

namespace MiniProjekt
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string personalInfoQueue = "personal_info_queue";
            string luggageQueue = "luggage_queue";
            string outputQueue = "output_queue";

            channel.QueueDeclare(queue: personalInfoQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: luggageQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: outputQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var producer = new Producer(channel, personalInfoQueue, luggageQueue);
            // Sti til hvor XML filen er gemt
            producer.SendMessages("/Users/mikkel/Documents/Datamatiker/4. semester/" +
                                  "Systemintegration/Systemintegration/MiniProjekt/FlightDetailsInfoResponse.xml");

            var resequencer = new Resequencer(channel, outputQueue);

            var personalInfoConsumer = new Consumer(channel, personalInfoQueue, luggageQueue, resequencer);
            personalInfoConsumer.Start();

            var luggageConsumer = new Consumer(channel, personalInfoQueue, luggageQueue, resequencer);
            luggageConsumer.Start();

            var aggregator = new Aggregator(channel, outputQueue);
            aggregator.Start();
            
            
            Thread.Sleep(3000);
            
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}