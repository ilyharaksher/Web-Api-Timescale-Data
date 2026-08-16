namespace Web_Api_Timescale_Data.Exceptions
{
    public class CsvValidationException : Exception
    {
        public CsvValidationException(string message) : base(message)
        {
        }
    }
}
