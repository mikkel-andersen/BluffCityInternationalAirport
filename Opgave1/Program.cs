using System;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using Opgave1.Models;
using Opgave1.Recipients;
using Opgave1.RabbitMQ;

namespace Opgave1
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlData = File.ReadAllText("/Users/mikkel/Documents/Datamatiker/4. semester/Systemintegration/Systemintegration/Opgave1/arrival_info.xml");
            CBPArrivalInfo arrivalInfo = DeserializeXml<CBPArrivalInfo>(xmlData);

            RecipientList recipientList = new RecipientList();
            recipientList.ProcessArrivalInfo(arrivalInfo);

            // Start the consumer to receive messages for each nationality
            var nationalities = arrivalInfo.Passports.Select(p => p.Nationality).Distinct();
            Consumer.StartConsumersForNationalities(nationalities);
        }

        static T DeserializeXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}