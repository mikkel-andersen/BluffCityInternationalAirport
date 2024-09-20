using System;
using RabbitMQ.Client;

namespace BluffCityInformationCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "PersonalInfoQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: "LuggageQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var producer = new Producer(channel, "PersonalInfoQueue", "LuggageQueue");
                producer.SendMessages(@"/Users/mikkel/Documents/Datamatiker/4. semester/Systemintegration/Systemintegration/Splitter/XML/FlightInfo.xml");

                var personalInfoConsumer = new Consumer(channel, "PersonalInfoQueue");
                personalInfoConsumer.ReceiveMessages();

                var luggageConsumer = new Consumer(channel, "LuggageQueue");
                luggageConsumer.ReceiveMessages();

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}