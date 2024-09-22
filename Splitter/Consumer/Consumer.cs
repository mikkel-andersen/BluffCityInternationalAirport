using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Splitter.Resequencer;

namespace BluffCityInformationCenter
{
    public class Consumer
    {
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly Resequencer _resequencer;
        private readonly string _messageKey;

        public Consumer(IModel channel, string queueName, Resequencer resequencer, string messageKey)
        {
            _channel = channel;
            _queueName = queueName;
            _resequencer = resequencer;
            _messageKey = messageKey;
        }

        public void ReceiveMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", message);
                _resequencer.AddMessage(_messageKey, message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
    }
}