namespace Web_Api_Timescale_Data.Entities
{
    public class ValueEntity
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public DateTime Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }

    }
}
