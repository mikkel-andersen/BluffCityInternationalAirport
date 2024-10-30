namespace Canonical
{

    public class SASTransformer : ITransformer
    {
        private readonly AirlineCompanySAS _sasData;

        public SASTransformer(AirlineCompanySAS sasData)
        {
            _sasData = sasData;
        }

        public AirlineCompany Transform()
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
}