using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;

namespace RequestReply.Messaging
{
    public class Consumer
    {
        private readonly string _requestQueueName;
        private readonly string _invalidQueueName;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Consumer(string requestQueueName, string invalidQueueName, IConnection connection)
        {
            _requestQueueName = requestQueueName;
            _invalidQueueName = invalidQueueName;
            _connection = connection;
            _channel = _connection.CreateModel();
            // Declare the request and invalid queues
            _channel.QueueDeclare(queue: _requestQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _invalidQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnReceiveCompleted;
            // Start consuming messages from the request queue
            _channel.BasicConsume(queue: _requestQueueName, autoAck: true, consumer: consumer);

            Console.WriteLine(" [x] Awaiting RPC requests");
            while (true)
            {
                Task.Delay(1000).Wait();
            }
        }

        private void OnReceiveCompleted(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

            try
            {
                // Log the received request details
                Console.WriteLine("Received request");
                Console.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
                Console.WriteLine("\tCorrel. ID: {0}", ea.BasicProperties.CorrelationId);
                Console.WriteLine("\tReply to:   {0}", ea.BasicProperties.ReplyTo);
                Console.WriteLine("\tContents:   {0}", message);

                string contents = message;
                if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.TryGetValue("Label", out var labelObj))
                {
                    var label = Encoding.UTF8.GetString((byte[])labelObj);
                    // Determine the response based on the label
                    switch (label)
                    {
                        case "SK":
                            contents = "13:45";
                            break;
                        case "KL":
                            contents = "14:25";
                            break;
                        case "SW":
                            contents = "15:40";
                            break;
                    }
                }

                var responseBytes = Encoding.UTF8.GetBytes(contents);
                // Publish the response to the reply queue
                _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProps, body: responseBytes);

                // Log the sent reply details
                Console.WriteLine("Sent reply");
                Console.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
                Console.WriteLine("\tCorrel. ID: {0}", ea.BasicProperties.CorrelationId);
                Console.WriteLine("\tContents:   {0}", contents);
            }
            catch (Exception)
            {
                // Handle invalid messages
                Console.WriteLine("Invalid message detected");
                _channel.BasicPublish(exchange: "", routingKey: _invalidQueueName, basicProperties: replyProps, body: body);
                Console.WriteLine("Sent to invalid message queue");
            }
        }
    }
}