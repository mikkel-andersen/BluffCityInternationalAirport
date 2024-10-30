using Opgave1.Models;
using Opgave1.RabbitMQ;
using System.Collections.Generic;

namespace Opgave1.Recipients
{
    public class RecipientList
    {
        public void ProcessArrivalInfo(CBPArrivalInfo arrivalInfo)
        {
            foreach (var passport in arrivalInfo.Passports)
            {
                Producer.SendMessage(passport);
            }
        }
    }
}