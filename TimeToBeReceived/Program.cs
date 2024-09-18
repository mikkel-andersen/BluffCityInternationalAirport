using RabbitMQ.Client;

namespace TimeToBeReceived
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string queueName = "TestQueue";

                // Declare the queue
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                // Initialize Producer
                var producer = new Producer(channel, queueName);
                producer.SendMessage("Test Message", 5000);

                // Wait for 6 seconds to ensure the message expires
                Console.WriteLine("Waiting for 6 seconds...");
                System.Threading.Thread.Sleep(6000);

                // Initialize Consumer
                var consumer = new Consumer(channel, queueName);
                consumer.ReceiveMessage();
            }
        }
    }
}