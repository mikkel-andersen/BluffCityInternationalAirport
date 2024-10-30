using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Opgave1.Models
{
    [XmlRoot("CBPArrivalInfo")]
    public class CBPArrivalInfo
    {
        public Flight Flight { get; set; }
        public Passenger Passenger { get; set; }
        [XmlElement("Passport")]
        public List<Passport> Passports { get; set; }
    }

    public class Flight
    {
        [XmlAttribute("number")]
        public string Number { get; set; }
        [XmlAttribute("Flightdate")]
        public string FlightDate { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }

    public class Passenger
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DayOfBirth { get; set; }
        public string Height { get; set; }
        public string Sex { get; set; }
    }

    public class Passport
    {
        public string PassNo { get; set; }
        public string Nationality { get; set; }
    }
}