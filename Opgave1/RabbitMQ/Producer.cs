using RabbitMQ.Client;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Opgave1.Models;

namespace Opgave1.RabbitMQ
{
    public static class Producer
    {
        public static void SendMessage(Passport passport)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: passport.Nationality,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                string message = SerializeToXml(passport);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: passport.Nationality,
                    basicProperties: null,
                    body: body);
            }
        }

        private static string SerializeToXml(Passport passport)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Passport));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, passport);
                return writer.ToString();
            }
        }
    }
}