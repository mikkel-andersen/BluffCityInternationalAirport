using Systemintegration.Messaging;
using System.Threading.Tasks;

namespace Systemintegration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var messageSAS = new
            {
                Header = new
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    Sender = "Airport Information Center",
                    Receiver = "Scandinavian Airline Service"
                },
                Body = new
                {
                    Airline = "Scandinavian Airline Service",
                    ScheduledTime = "2023-10-01T15:30:00Z",
                    FlightNo = "SK123",
                    Destination = "Copenhagen",
                    CheckIn = "Terminal 1, Counter 5"
                }
            };

            var messageSWA = new
            {
                Header = new
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    Sender = "Airport Information Center",
                    Receiver = "South West Airlines"
                },
                Body = new
                {
                    Airline = "South West Airlines",
                    ScheduledTime = "2023-10-01T16:00:00Z",
                    FlightNo = "SW456",
                    Destination = "New York",
                    CheckIn = "Terminal 2, Counter 10"
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
                    CheckIn = "Terminal 4, Counter 15"
                }
            };
            
            

            var producer = new Producer("AirportInformationCenterQueue");
            producer.SendMessage(messageSAS);
            producer.SendMessage(messageSWA);
            producer.SendMessage(messageKLM);

            var router = new SimpleRouter("AirportInformationCenterQueue", "SASQueue", "SWQueue", "KLMQueue");
            await router.RouteMessages();

            var consumerSAS = new Consumer("SASQueue");
            var consumerSWA = new Consumer("SWQueue");
            var consumerKLM = new Consumer("KLMQueue");

            await Task.WhenAll(consumerSAS.ReceiveMessages(), consumerSWA.ReceiveMessages(), consumerKLM.ReceiveMessages());
        }
    }
}