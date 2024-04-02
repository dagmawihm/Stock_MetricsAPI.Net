namespace StockMetricsAPI.Dtos
{
    public class DayDataDto
    {
        public required string Date { get; set; }
        public double Close { get; set; }
    }
}
