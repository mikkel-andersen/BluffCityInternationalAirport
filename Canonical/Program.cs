using Canonical;

class Program
{
    static void Main(string[] args)
    {
        var sasData = new AirlineCompanySAS
        {
            Airline = "SAS",
            FlightNo = "SK239",
            Destination = "JFK",
            Origin = "CPH",
            ArrivalDeparture = "D",
            Date = new DateTime(2017, 3, 6),
            Time = new TimeSpan(16, 45, 0)
        };

        ITransformer transformer = TransformerFactory.GetTransformer(sasData);
        AirlineCompany canonicalData = transformer.Transform();

        Console.WriteLine($"Airline: {canonicalData.Airline}" +
                          $"\nFlightNo: {canonicalData.FlightNo}" +
                          $"\nStatus: {canonicalData.ArrivalDeparture}" +
                          $"\nDestination: {canonicalData.Destination}" +
                          $"\nOrigin: {canonicalData.Origin}" +
                          $"\nDateTime: {canonicalData.DateTime}");
    }
}