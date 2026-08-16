namespace Web_Api_Timescale_Data.Entities
{
    public class ResultEntity
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public double Delta { get; set; } // дельта времени Date в секундах(максимальное Date – минимальное Date)
        public DateTime MinDate { get; set; } // минимальное дата и время, как момент запуска первой операции (Date)
        public double AvgExecutionTime { get; set; } // среднее время выполнения (ExecutionTime)
        public double AvgValue { get; set; } // среднее значение по показателям (Value)
        public double MedianValue { get; set; } //  медина по показателям (Value
        public double MaxValue { get; set; } // максимальное значение показателя (Value)
        public double MinValue { get; set; } //  минимальное значение показателя (Value)
    }
}
