namespace AdapterLøsning;

public class SASAdapter : IAdapter
{
    private readonly AirlineCompanySAS _sasData;

    public SASAdapter(AirlineCompanySAS sasData)
    {
        _sasData = sasData;
    }

    public AirlineCompany GetCanonicalModel()
    {
        return new AirlineCompany
        {
            Airline = _sasData.Airline,
            FlightNo = _sasData.FlightNo,
            Destination = _sasData.Destination,
            Origin = _sasData.Origin,
            ArrivalDeparture = _sasData.ArrivalDeparture,
            DateTime = _sasData.Date.Add(_sasData.Time)
        };
    }
}