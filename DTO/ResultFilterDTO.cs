namespace Web_Api_Timescale_Data.DTO
{
    public class ResultFilterDTO
    {
        public string? FileName { get; set; }

        public DateTime? MinStartDate { get; set; }
        public DateTime? MaxStartDate { get; set; }

        public double? MinAvgValue { get; set; }
        public double? MaxAvgValue { get; set; }

        public double? MinAvgTime { get; set; }
        public double? MaxAvgTime { get; set; }
    }
}
