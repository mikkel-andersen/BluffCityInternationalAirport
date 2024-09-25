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

        public void SendMessages(string filePath)
        {
            XElement flightDetails = XElement.Load(filePath);
            string messageId = Guid.NewGuid().ToString();

            // Beregn antal sequences der sendes
            int totalSequences = 1 + flightDetails.Elements("Luggage").Count();

            // Send personal info
            var personalInfo = new XElement("PersonalInfoMessage",
                new XElement("MessageId", messageId),
                new XElement("SequenceNumber", 1),
                new XElement("TotalSequences", totalSequences),
                flightDetails.Element("Flight"),
                flightDetails.Element("Passenger"));
            SendMessage(_personalInfoQueue, personalInfo.ToString());

            // Send luggage info
            int sequenceNumber = 2;
            foreach (var luggage in flightDetails.Elements("Luggage"))
            {
                var luggageInfo = new XElement("LuggageMessage",
                    new XElement("MessageId", messageId),
                    new XElement("SequenceNumber", sequenceNumber++),
                    new XElement("TotalSequences", totalSequences),
                    luggage);
                SendMessage(_luggageQueue, luggageInfo.ToString());
            }
        }

        private void SendMessage(string queueName, string messageBody)
        {
            var body = Encoding.UTF8.GetBytes(messageBody);
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            Console.WriteLine("Message sent to {0}", queueName);
        }
    }
}