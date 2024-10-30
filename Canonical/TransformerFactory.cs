namespace Canonical
{

    public static class TransformerFactory
    {
        public static ITransformer GetTransformer(object data)
        {
            if (data is AirlineCompanySAS sasData)
            {
                return new SASTransformer(sasData);
            }

            throw new ArgumentException("Unsupported data type");
        }
    }
}