using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace MiniProjekt
{
    public class Resequencer
    {
        private readonly IModel _channel;
        private readonly string _outputQueue;
        private readonly ConcurrentDictionary<int, string> _messages = new ConcurrentDictionary<int, string>();

        public Resequencer(IModel channel, string outputQueue)
        {
            _channel = channel;
            _outputQueue = outputQueue;
        }

        public void AddMessage(int sequenceNumber, string message)
        {
            _messages[sequenceNumber] = message;
            Console.WriteLine("Message added with sequence number: {0}", sequenceNumber);
            if (_messages.Count >= 3) // Baseret på at XML filen deles i 3.
            {
                SendOrderedMessages();
            }
        }

        private void SendOrderedMessages()
        {
            try
            {
                foreach (var message in _messages.OrderBy(m => m.Key))
                {
                    var body = Encoding.UTF8.GetBytes(message.Value);
                    _channel.BasicPublish(exchange: "", routingKey: _outputQueue, basicProperties: null, body: body);
                }
                Console.WriteLine("Ordered messages sent to {0}", _outputQueue);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending ordered messages: {0}", ex.Message);
            }
        }
    }
}