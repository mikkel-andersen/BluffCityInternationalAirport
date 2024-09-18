using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OfficeOpenXml;
using System.IO;

namespace Adapter
{
    public class Consumer
    {
        private readonly RabbitMQ.Client.IModel _channel;
        private readonly string _queueName;

        public Consumer(RabbitMQ.Client.IModel channel, string queueName)
        {
            _channel = channel;
            _queueName = queueName;
        }

        public void ReceiveMessage()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", message);
                WriteToExcel(message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        private void WriteToExcel(string message)
        {
            try
            {
                // Set the license context for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var filePath = @"/Users/mikkel/Desktop/Excel.xlsx"; // Corrected file path
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Ensure the workbook has at least one worksheet
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        package.Workbook.Worksheets.Add("Sheet1");
                    }

                    var worksheet = package.Workbook.Worksheets[0];

                    // Find the next empty row
                    int row = 1;
                    while (!string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                    {
                        row++;
                    }

                    // Split the message into FlightNo and Estimated Time of Arrival
                    var parts = message.Split(',');
                    if (parts.Length == 2)
                    {
                        worksheet.Cells[row, 1].Value = parts[0]; // FlightNo
                        worksheet.Cells[row, 2].Value = parts[1]; // Estimated Time of Arrival
                        Console.WriteLine("Written to Excel: FlightNo={0}, ETA={1}", parts[0], parts[1]);
                    }
                    else
                    {
                        Console.WriteLine("Invalid message format: {0}", message);
                    }

                    package.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to Excel: {0}", ex.Message);
            }
        }
    }
}