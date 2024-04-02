namespace StockMetricsAPI.Dtos
{
    public class ReturnDto
    {
        public required string date { get; set; }
        public double decimalReturn { get; set; }
        public required string percentileReturn { get; set; }
    }
}
