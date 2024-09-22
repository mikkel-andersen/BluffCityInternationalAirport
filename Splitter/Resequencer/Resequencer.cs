using System.Text;
using RabbitMQ.Client;

namespace Splitter.Resequencer
{
    public class Resequencer
    {
        private readonly IModel _channel;
        private readonly string _outputQueue;
        private readonly Dictionary<string, string> _messages = new Dictionary<string, string>();

        public Resequencer(IModel channel, string outputQueue)
        {
            _channel = channel;
            _outputQueue = outputQueue;
        }

        public void AddMessage(string key, string message)
        {
            _messages[key] = message;
            if (_messages.ContainsKey("PersonalInfo") && _messages.ContainsKey("LuggageInfo1") && _messages.ContainsKey("LuggageInfo2"))
            {
                SendCombinedMessage();
            }
        }

        private void SendCombinedMessage()
        {
            var combinedMessage = new StringBuilder();
            combinedMessage.Append(_messages["PersonalInfo"]);
            combinedMessage.Append(_messages["LuggageInfo1"]);
            combinedMessage.Append(_messages["LuggageInfo2"]);

            var body = Encoding.UTF8.GetBytes(combinedMessage.ToString());
            _channel.BasicPublish(exchange: "", routingKey: _outputQueue, basicProperties: null, body: body);
            Console.WriteLine("Combined message sent to {0}", _outputQueue);
        }
    }
}