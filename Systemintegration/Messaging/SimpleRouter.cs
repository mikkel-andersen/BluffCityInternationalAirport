using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Systemintegration.Messaging
{
    public class SimpleRouter
    {
        private readonly string _inputQueueName;
        private readonly string _outputQueueName1;
        private readonly string _outputQueueName2;
        private readonly string _outputQueueName3;
        private readonly IConnection _connection;

        public SimpleRouter(string inputQueueName, string outputQueueName1, string outputQueueName2, string outputQueueName3, IConnection connection)
        {
            _inputQueueName = inputQueueName;
            _outputQueueName1 = outputQueueName1;
            _outputQueueName2 = outputQueueName2;
            _outputQueueName3 = outputQueueName3;
            _connection = connection;
        }

        public async Task RouteMessages()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: _inputQueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var deserializedMessage = JObject.Parse(message);
                    var receiver = deserializedMessage["Header"]["Receiver"].ToString();

                    if (receiver == "SAS")
                    {
                        SendMessageToQueue(_outputQueueName1, message);
                    }
                    else if (receiver == "SWA")
                    {
                        SendMessageToQueue(_outputQueueName2, message);
                    }
                    else if (receiver == "KLM")
                    {
                        SendMessageToQueue(_outputQueueName3, message);
                    }
                    else
                    { 
                        Console.WriteLine("Unknown receiver: {0}", receiver);
                    }
                };
                channel.BasicConsume(queue: _inputQueueName,
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private void SendMessageToQueue(string queueName, string message)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" [x] Routed {0} to {1}", message, queueName);
            }
        }
    }
}