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

        public void SendMessage(string messageBody, int expiration)
        {
            var body = Encoding.UTF8.GetBytes(messageBody);
            var properties = _channel.CreateBasicProperties();
            properties.Expiration = expiration.ToString();

            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: properties, body: body);
            Console.WriteLine("Message sent with expiration set to {0} milliseconds.", expiration);
        }
    }
}