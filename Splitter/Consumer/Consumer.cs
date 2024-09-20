using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BluffCityInformationCenter
{
    public class Consumer
    {
        private readonly IModel _channel;
        private readonly string _queueName;

        public Consumer(IModel channel, string queueName)
        {
            _channel = channel;
            _queueName = queueName;
        }

        public void ReceiveMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
    }
}