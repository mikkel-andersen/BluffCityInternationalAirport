using System;
using System.Text;
using System.Xml.Linq;
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
                channel.QueueDeclare(queue: "AirportInfoGateNo",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                SendMessage(channel, @"AirportInforGateNoSAS.xml", "SAS");
                SendMessage(channel, @"AirportInforGateNoKLM.xml", "KLM");
                SendMessage(channel, @"AirportInforGateNoSW.xml", "SW");
            }
        }

        private static void SendMessage(IModel channel, string filePath, string airlineCompany)
        {
            XElement booksFromFile = XElement.Load(filePath);
            Console.WriteLine(booksFromFile);

            var messageBody = Encoding.UTF8.GetBytes(booksFromFile.ToString());
            var properties = channel.CreateBasicProperties();
            properties.Headers = new System.Collections.Generic.Dictionary<string, object>
            {
                { "AirlineCompany", airlineCompany }
            };

            channel.BasicPublish(exchange: "",
                routingKey: "AirportInfoGateNo",
                basicProperties: properties,
                body: messageBody);

            Console.WriteLine(" [x] Sent {0}", airlineCompany);
        }
    }
}