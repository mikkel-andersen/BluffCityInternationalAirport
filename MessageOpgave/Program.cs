using System;
using System.Threading.Tasks;
using Systemintegration.Messaging;
using RabbitMQ.Client;

namespace Systemintegration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                var messageSAS = new
                {
                    Header = new
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        Sender = "Airport Information Center",
                        Receiver = "SAS"
                    },
                    Body = new
                    {
                        Airline = "Scandinavian Airline Service",
                        ScheduledTime = "2023-10-01T15:30:00Z",
                        FlightNo = "SK123",
                        Destination = "Copenhagen",
                        CheckIn = "Terminal 1, Counter 5",
                        Gate = "Gate 1"
                    }
                };

                var messageSWA = new
                {
                    Header = new
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        Sender = "Airport Information Center",
                        Receiver = "SWA"
                    },
                    Body = new
                    {
                        Airline = "South West Airlines",
                        ScheduledTime = "2023-10-01T16:00:00Z",
                        FlightNo = "SW456",
                        Destination = "New York",
                        CheckIn = "Terminal 2, Counter 10",
                        Gate = "Gate 3"
                    }
                };

                var messageKLM = new
                {
                    Header = new
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        Sender = "Airport Information Center",
                        Receiver = "KLM"
                    },
                    Body = new
                    {
                        Airline = "KLM",
                        ScheduledTime = "2023-10-01T16:00:00Z",
                        FlightNo = "KLM744",
                        Destination = "Paris",
                        CheckIn = "Terminal 4, Counter 15",
                        Gate = "Gate 8"
                    }
                };

                var producer = new Producer("AirportInformationCenterQueue", connection);
                producer.SendMessage(messageSAS);
                producer.SendMessage(messageSWA);
                producer.SendMessage(messageKLM);

                var router = new SimpleRouter("AirportInformationCenterQueue", "SASQueue", "SWQueue", "KLMQueue", connection);
                await router.RouteMessages();

                var consumerSAS = new Consumer("SASQueue", connection);
                var consumerSWA = new Consumer("SWQueue", connection);
                var consumerKLM = new Consumer("KLMQueue", connection);

                await Task.WhenAll(consumerSAS.ReceiveMessages(), consumerSWA.ReceiveMessages(), consumerKLM.ReceiveMessages());
            }
        }
    }
}