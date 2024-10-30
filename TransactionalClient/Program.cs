using RabbitMQ.Client;
using TransactionalClient.RabbitMQ;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Create and use the producer to send messages
            var producer = new Producer(channel, "passengerQueue", "luggageQueue");
            producer.SendMessages("/Users/mikkel/Documents/Datamatiker/4. semester/Systemintegration/Systemintegration/TransactionalClient/XML.xml");

            // Create and use the consumer to receive messages
            Consumer.ReceiveMessages();
        }
    }
}