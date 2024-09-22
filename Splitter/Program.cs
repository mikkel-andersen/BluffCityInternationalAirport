using BluffCityInformationCenter;
using RabbitMQ.Client;

namespace Splitter
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
                channel.QueueDeclare(queue: "OutputQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var resequencer = new Resequencer.Resequencer(channel, "OutputQueue");

                var producer = new Producer(channel, "PersonalInfoQueue", "LuggageQueue");
                producer.SendMessages(@"/Users/mikkel/Documents/Datamatiker/4. semester/Systemintegration/Systemintegration/Splitter/XML/FlightInfo.xml");

                var personalInfoConsumer = new Consumer(channel, "PersonalInfoQueue", resequencer, "PersonalInfo");
                personalInfoConsumer.ReceiveMessages();

                var luggageConsumer1 = new Consumer(channel, "LuggageQueue", resequencer, "LuggageInfo1");
                luggageConsumer1.ReceiveMessages();

                var luggageConsumer2 = new Consumer(channel, "LuggageQueue", resequencer, "LuggageInfo2");
                luggageConsumer2.ReceiveMessages();

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}