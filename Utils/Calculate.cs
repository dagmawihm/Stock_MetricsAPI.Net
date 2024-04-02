using StockMetricsAPI.Dtos;

namespace StockMetricsAPI.Utils
{
   public class Calculate
    {
        // Calculate daily returns
        public static double DailyReturn(double close, double previousClose)
        {
            return ((close / previousClose) - 1);
        }

        public static List<AlphaDto> Alpha(double[] stockReturns, double[] benchmarkReturns, string[] dates)
        {
            var alphaResults = new List<AlphaDto>();

            // Calculate differences
            for (int i = 0; i < stockReturns.Length; i++)
            {
                var result = stockReturns[i] - benchmarkReturns[i];
                var date = dates[i];
                alphaResults.Add(new AlphaDto { Result = result, Date = date });
            }

            return alphaResults;
        }



    }
}

