using System;
using System.Threading.Tasks;
using Alternativ_Til_Publisher_Subscriber;
using RabbitMQ.Client;

namespace Systemintegration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var etaMessage = new
            {
                Header = new
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    Sender = "Air Traffic Control",
                    Receiver = "Airport Information Center"
                },
                Body = new
                {
                    FlightNr = "SA335",
                    Airline = "SAS",
                    ScheduledArrivalTime = DateTime.UtcNow.AddHours(2).ToString("HH:mm:ss"),
                    EstimatedArrivalTime = DateTime.UtcNow.AddHours(2).AddMinutes(30).ToString("HH:mm:ss"),
                    Origin = "London",
                    Destination = "Bluff City"
                }
            };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                var channel = connection.CreateModel();

                // Declare the exchange
                channel.ExchangeDeclare(exchange: "ETAExchange", type: "fanout");

                var producer = new Producer("ETAExchange", connection);
                producer.SendMessage(etaMessage);

                channel.QueueDeclare(queue: "SASQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: "SWAQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: "KLMQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                channel.QueueBind(queue: "SASQueue", exchange: "ETAExchange", routingKey: "");
                channel.QueueBind(queue: "SWAQueue", exchange: "ETAExchange", routingKey: "");
                channel.QueueBind(queue: "KLMQueue", exchange: "ETAExchange", routingKey: "");

                var consumerSAS = new Consumer("SASQueue", connection, "SAS");
                var consumerSWA = new Consumer("SWAQueue", connection, "SW");
                var consumerKLM = new Consumer("KLMQueue", connection, "KLM");

                await Task.WhenAll(consumerSAS.ReceiveMessages(), consumerSWA.ReceiveMessages(), consumerKLM.ReceiveMessages());
            }
        }
    }
}