using System.Text;
using RabbitMQ.Client;

namespace TimeToBeReceived
{
    public class Producer
    {
        private readonly IModel _channel;
        private readonly string _queueName;

        public Producer(IModel channel, string queueName)
        {
            _channel = channel;
            _queueName = queueName;
        }

        public void SendMessage(string messageBody)
        {
            var body = Encoding.UTF8.GetBytes(messageBody);
            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine("Message sent: {0}", messageBody);
        }
    }
}