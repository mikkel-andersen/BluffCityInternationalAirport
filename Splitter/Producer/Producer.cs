using System;
using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace BluffCityInformationCenter
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

            // Send personal info
            var personalInfo = new XElement("PersonalInfoMessage",
                new XElement("MessageId", messageId),
                new XElement("SequenceNumber", 1),
                flightDetails.Element("Passenger"),
                flightDetails.Element("Flight"));
            SendMessage(_personalInfoQueue, personalInfo.ToString());

            // Send luggage info
            var luggageElements = flightDetails.Elements("Luggage");
            int sequenceNumber = 2;
            foreach (var luggage in luggageElements)
            {
                var luggageInfo = new XElement("LuggageMessage",
                    new XElement("MessageId", messageId),
                    new XElement("SequenceNumber", sequenceNumber++),
                    luggage);
                SendMessage(_luggageQueue, luggageInfo.ToString());
            }
        }

        private void SendMessage(string queueName, string messageBody)
        {
            var body = Encoding.UTF8.GetBytes(messageBody);
            var properties = _channel.CreateBasicProperties();

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
            Console.WriteLine("Message sent to {0}", queueName);
        }
    }
}