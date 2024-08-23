using Systemintegration.Messaging;
using System.Threading.Tasks;

namespace Systemintegration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var producer = new Producer("Airport Information Center");
            
            producer.SendMessage("Hej tully!");
            
            

            var consumer = new Consumer("Airport Information Center");
            await consumer.ReceiveMessages();
        }
    }
}