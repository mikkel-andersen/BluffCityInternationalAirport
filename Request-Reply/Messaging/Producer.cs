using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;

namespace RequestReply.Messaging
{
    public class Producer
    {
        private readonly string _requestQueueName;
        private readonly string _replyQueueName;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Producer(string requestQueueName, string replyQueueName, IConnection connection)
        {
            _requestQueueName = requestQueueName;
            _replyQueueName = replyQueueName;
            _connection = connection;
            _channel = _connection.CreateModel();
            // Declare the request and reply queues
            _channel.QueueDeclare(queue: _requestQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _replyQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void Send(string airline)
        {
            // Create the message object
            var message = new { Airline = airline };
            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            // Set the properties for the message
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            props.CorrelationId = Guid.NewGuid().ToString();
            props.Headers = new Dictionary<string, object> { { "Label", airline.Substring(0, 2) } };

            // Publish the message to the request queue
            _channel.BasicPublish(exchange: "", routingKey: _requestQueueName, basicProperties: props, body: messageBytes);

            // Log the sent message details
            Console.WriteLine("Sent request");
            Console.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
            Console.WriteLine("\tCorrel. ID: {0}", props.CorrelationId);
            Console.WriteLine("\tReply to:   {0}", props.ReplyTo);
            Console.WriteLine("\tContents:   {0}", airline);
        }

        public void ReceiveSync()
        {
            var consumer = new EventingBasicConsumer(_channel);
            string response = null;

            // Event handler for receiving messages
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId != null)
                {
                    response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    // Log the received message details
                    Console.WriteLine("Received reply");
                    Console.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
                    Console.WriteLine("\tCorrel. ID: {0}", ea.BasicProperties.CorrelationId);
                    Console.WriteLine("\tContents:   {0}", response);
                }
            };

            // Start consuming messages from the reply queue
            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: consumer);

            // Wait for the response
            while (response == null)
            {
                Task.Delay(100).Wait();
            }
        }
    }
}