using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace TransactionalClient.RabbitMQ
{
    public class Producer
    {
        private readonly IModel _channel;
        private readonly string _passengerQueue;
        private readonly string _luggageQueue;

        public Producer(IModel channel, string passengerQueue, string luggageQueue)
        {
            _channel = channel;
            _passengerQueue = passengerQueue;
            _luggageQueue = luggageQueue;
        }

        public void SendMessages(string filePath)
        {
            XElement flightDetails = XElement.Load(filePath);
            var messages = new List<(string Queue, string Message)>();

            try
            {
                // Build passenger info message
                var passengerInfo = new XElement("PassengerInfoMessage",
                    flightDetails.Element("Flight"),
                    flightDetails.Element("Passenger"));
                messages.Add((_passengerQueue, passengerInfo.ToString()));

                // Build luggage info messages
                foreach (var luggage in flightDetails.Elements("Luggage"))
                {
                    var luggageInfo = new XElement("LuggageMessage", luggage);
                    messages.Add((_luggageQueue, luggageInfo.ToString()));
                }

                _channel.TxSelect(); // Start transaction

                // Send all messages within a single transaction
                foreach (var (queue, message) in messages)
                {
                    SendMessage(queue, message);
                }

                // Simulate an error to trigger rollback
                throw new InvalidOperationException("Simulated error to trigger rollback");

                _channel.TxCommit(); // Commit transaction
            }
            catch (Exception)
            {
                _channel.TxRollback(); // Rollback transaction in case of error
                throw;
            }
        }

        private void SendMessage(string queueName, string messageBody)
        {
            var body = Encoding.UTF8.GetBytes(messageBody);
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}