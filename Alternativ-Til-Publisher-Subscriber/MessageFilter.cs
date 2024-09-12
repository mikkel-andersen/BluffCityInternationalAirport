
using System.Text.Json;
namespace Alternativ_Til_Publisher_Subscriber
{
    public class MessageFilter
    {
        private readonly string _airline;

        public MessageFilter(string airline)
        {
            _airline = airline;
        }

        public bool FilterMessage(string message)
        {
            var etaMessage = JsonSerializer.Deserialize<ETA>(message);
            return etaMessage.Body.Airline == _airline;
        }
    }

    public class ETA
    {
        public Header Header { get; set; }
        public Body Body { get; set; }
    }

    public class Header
    {
        public string MessageId { get; set; }
        public string Timestamp { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }

    public class Body
    {
        public string FlightNr { get; set; }
        public string Airline { get; set; }
        public string ScheduledArrivalTime { get; set; }
        public string EstimatedArrivalTime { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}