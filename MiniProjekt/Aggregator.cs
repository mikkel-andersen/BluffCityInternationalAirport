using System;
using System.Text;
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
        private readonly int _expectedMessageCount = 3; // Baseret på at XML filen opdeles i tre
        

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
                _receivedMessageCount++; // Tæller op
                //Console.WriteLine("Received part of combined message: {0}", message); // Printer den del der er modtaget

                if (_receivedMessageCount >= _expectedMessageCount)
                {
                    Console.WriteLine("Received combined message: {0}", _combinedMessage.ToString()); // Printer den samlede besked
                    _combinedMessage.Clear(); // Rydder den samlede besked
                    _receivedMessageCount = 0; // Resetter så den er klar til næste besked
                }
            };
            _channel.BasicConsume(queue: _outputQueue, autoAck: true, consumer: consumer);
        }
    }
}