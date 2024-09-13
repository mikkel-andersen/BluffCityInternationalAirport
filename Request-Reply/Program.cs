using System;
using RabbitMQ.Client;
using RequestReply.Messaging;
using System.Threading.Tasks;

namespace RequestReply
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                string requestQueue = "BluffCityRequestQueueAIC";
                string replyQueueSAS = "BluffCityReplyQueueSAS";
                string replyQueueSW = "BluffCityReplyQueueSW";
                string replyQueueKLM = "BluffCityReplyQueueKLM";
                string invalidQueue = "InvalidQueue";

                var consumer = new Consumer(requestQueue, invalidQueue, connection);
                Task.Run(() => consumer.StartListening());

                var producerSAS = new Producer(requestQueue, replyQueueSAS, connection);
                var producerSW = new Producer(requestQueue, replyQueueSW, connection);
                var producerKLM = new Producer(requestQueue, replyQueueKLM, connection);

                producerSAS.Send("SK249");
                producerSAS.ReceiveSync();
                producerKLM.Send("KLM582");
                producerKLM.ReceiveSync();
                producerSW.Send("SW1423");
                producerSW.ReceiveSync();
            }
        }
    }
}