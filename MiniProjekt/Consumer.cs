using System;
using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MiniProjekt
{
    public class Consumer
    {
        private readonly IModel _channel;
        private readonly string _personalInfoQueue;
        private readonly string _luggageQueue;
        private readonly Resequencer _resequencer;

        public Consumer(IModel channel, string personalInfoQueue, string luggageQueue, Resequencer resequencer)
        {
            _channel = channel;
            _personalInfoQueue = personalInfoQueue;
            _luggageQueue = luggageQueue;
            _resequencer = resequencer;
        }

        public void Start()
        {
            var personalInfoConsumer = new EventingBasicConsumer(_channel);
            personalInfoConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var xmlMessage = XElement.Parse(message);
                var sequenceNumber = int.Parse(xmlMessage.Element("SequenceNumber").Value);
                var totalSequences = int.Parse(xmlMessage.Element("TotalSequences").Value);
                _resequencer.AddMessage(sequenceNumber, totalSequences, message);
            };
            _channel.BasicConsume(queue: _personalInfoQueue, autoAck: true, consumer: personalInfoConsumer);

            var luggageConsumer = new EventingBasicConsumer(_channel);
            luggageConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var xmlMessage = XElement.Parse(message);
                var sequenceNumber = int.Parse(xmlMessage.Element("SequenceNumber").Value);
                var totalSequences = int.Parse(xmlMessage.Element("TotalSequences").Value);
                _resequencer.AddMessage(sequenceNumber, totalSequences, message);
            };
            _channel.BasicConsume(queue: _luggageQueue, autoAck: true, consumer: luggageConsumer);
        }
    }
}