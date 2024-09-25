using System;
using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MiniProjekt
{
    public class Aggregator
    {
        private readonly IModel _channel;
        private readonly string _outputQueue;
        private readonly StringBuilder _combinedMessage = new StringBuilder();
        private int _receivedMessageCount = 0;
        private int _expectedMessageCount = 0;

        public Aggregator(IModel channel, string outputQueue)
        {
            _channel = channel;
            _outputQueue = outputQueue;
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _combinedMessage.Append(message);
                _receivedMessageCount++;

                if (_expectedMessageCount == 0)
                {
                    _expectedMessageCount = int.Parse(XElement.Parse(message).Element("TotalSequences").Value);
                }

                if (_receivedMessageCount >= _expectedMessageCount)
                {
                    Console.WriteLine("Received combined message: {0}", _combinedMessage.ToString());
                    _combinedMessage.Clear();
                    _receivedMessageCount = 0;
                    _expectedMessageCount = 0;
                }
            };
            _channel.BasicConsume(queue: _outputQueue, autoAck: true, consumer: consumer);
        }
    }
}