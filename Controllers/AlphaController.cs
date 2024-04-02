using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockMetricsAPI.Dtos;
using StockMetricsAPI.Utils;
using System.Net;

namespace StockMetricsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlphaController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAlpha([FromQuery] string symbol, [FromQuery] string? benchmark = null, [FromQuery] string? from = null, [FromQuery] string? to = null)
        {
            if (string.IsNullOrEmpty(benchmark))
            {
                return StatusCode(400, new { error = "Benchmark symbol is required" });// Ensure benchmark symbol is provided
            }

            // Construct URLs for fetching historical stock data and benchmark data from the return Endpoint
            string urlStock = $"http://localhost:5246/api/return?symbol={symbol}";
            string urlBenchmark = $"http://localhost:5246/api/return?symbol={benchmark}";

            // Append from and to date parameter to the URLs if provided in the request query
            if (!string.IsNullOrEmpty(from))
            {
                urlStock += $"&from={from}";
                urlBenchmark += $"&from={from}";
            }
            if (!string.IsNullOrEmpty(to))
            {
                urlStock += $"&to={to}";
                urlBenchmark += $"&to={to}";
            }

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var stockResponse = await httpClient.GetAsync(urlStock);
                    var benchmarkResponse = await httpClient.GetAsync(urlBenchmark);

                    if (stockResponse.IsSuccessStatusCode && benchmarkResponse.IsSuccessStatusCode)
                    {
                        // Make internal requests to get returns for the stock and the benchmark
                        string stockResponseBody = await stockResponse.Content.ReadAsStringAsync();
                        string benchmarkResponseBody = await benchmarkResponse.Content.ReadAsStringAsync();

                        // Deserialize the response body into a list of DayData objects
                        var stockData = JsonConvert.DeserializeObject<ReturnDataDto>(stockResponseBody);
                        var benchmarkData = JsonConvert.DeserializeObject<ReturnDataDto>(benchmarkResponseBody);

                        var stockReturns = stockData.returns.Select(item => item.decimalReturn);
                        var benchmarkReturns = benchmarkData.returns.Select(item => item.decimalReturn);

                        // Extracts the dates.
                        var dates = stockData.returns.Select(item => item.date);

                        // Check if both arrays have equal length

                        if (stockReturns.ToArray().Length != benchmarkReturns.ToArray().Length)
                        {
                            return StatusCode(400, new { error = "Data mismatch: The stock and benchmark data don't match up. Please check if both have data for the same time period." });
                        }

                        // Calculate alpha value using stock and benchmark returns data
                        var alpha = Calculate.Alpha(stockReturns.ToArray(), benchmarkReturns.ToArray(), dates.ToArray());
                        return Ok(new { alpha });// Respond with alpha value


                    }
                    else if (!stockResponse.IsSuccessStatusCode)
                    {
                        // Request failed, handle error on stock
                        var error = await stockResponse.Content.ReadAsStringAsync();
                        return StatusCode((int)HttpStatusCode.NotFound, error);
                    }
                    else
                    {
                        // Request failed, handle error on benchmark
                        var error = await benchmarkResponse.Content.ReadAsStringAsync();
                        return StatusCode((int)HttpStatusCode.NotFound, error );

                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
                }
            }
        }
    }
}



    
