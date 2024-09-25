using System;
using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace MiniProjekt
{
    public class Producer
    {
        private readonly IModel _channel;
        private readonly string _personalInfoQueue;
        private readonly string _luggageQueue;

        public Producer(IModel channel, string personalInfoQueue, string luggageQueue)
        {
            _channel = channel;
            _personalInfoQueue = personalInfoQueue;
            _luggageQueue = luggageQueue;
        }

        public void SendMessages(string filePath) // Hardcoded løsning
        {
            XElement flightDetails = XElement.Load(filePath);
            string messageId = Guid.NewGuid().ToString();

            // Send personal info
            var personalInfo = new XElement("PersonalInfoMessage",
                new XElement("MessageId", messageId),
                new XElement("SequenceNumber", 1), // Tilføjer et sequence nummer  til XML'en
                flightDetails.Element("Flight"),
                flightDetails.Element("Passenger"));
            SendMessage(_personalInfoQueue, personalInfo.ToString());

            // Send luggage info
            var luggageElements = flightDetails.Elements("Luggage");
            int sequenceNumber = 2; // Sequence nummer 2 fordi nummer 1 er givet til person.
            foreach (var luggage in luggageElements)
            {
                var luggageInfo = new XElement("LuggageMessage",
                    new XElement("MessageId", messageId),
                    new XElement("SequenceNumber", sequenceNumber++),
                    luggage);
                string luggageKey = sequenceNumber == 3 ? "LuggageInfo1" : "LuggageInfo2";
                SendMessage(_luggageQueue, luggageInfo.ToString(), luggageKey);
            }
        }

        private void SendMessage(string queueName, string messageBody, string luggageKey = null)
        {
            var body = Encoding.UTF8.GetBytes(messageBody);
            var properties = _channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> { { "LuggageKey", luggageKey } };

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
            Console.WriteLine("Message sent to {0}", queueName);
        }
    }
}