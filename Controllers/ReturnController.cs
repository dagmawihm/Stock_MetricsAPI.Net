using Microsoft.AspNetCore.Mvc;
using StockMetricsAPI.Dtos;
using DotNetEnv;
using Newtonsoft.Json;
using StockMetricsAPI.Utils;

namespace StockMetricsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetReturn([FromQuery] string symbol, [FromQuery] string? from = null, [FromQuery] string? to = null)
        {
            Env.Load();
            string tokenKey = Env.GetString("IEX_TOKEN"); // Retrieve IEX token from environment variables

            // Build URL: Historical data to max date if 'from' provided, else year-to-date.
            string url = from != null
                ? $"https://cloud.iexapis.com/stable/stock/{symbol}/chart/max?token={tokenKey}"
                : $"https://cloud.iexapis.com/stable/stock/{symbol}/chart/ytd?token={tokenKey}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);// Fetch historical prices from IEX API

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize the response body into a list of DayData objects
                        var dayData = JsonConvert.DeserializeObject<List<DayDataDto>>(responseBody);

                        if (string.IsNullOrEmpty(from))
                        {
                            // If 'from' date is null, set it to January 1st of the current year
                            from = $"{DateTime.Now.Year}-01-01";
                        }
                        string newFrom = from; // Initialize newFrom with from date to store the new starting date
                        
                        // Filter Data to get the new starting date
                        foreach (var daydata in dayData)
                        {
                            DateTime date = DateTime.Parse(daydata.Date);

                            if (dayData.IndexOf(daydata) > 0 && DateTime.Parse(from) <= date && newFrom == from)
                            {
                                newFrom = dayData[dayData.IndexOf(daydata) - 1].Date;
                            }
                        }

                        // Filter Data based on date range
                        var filteredDayData = dayData.Where(daydata =>
                        {
                            DateTime date = DateTime.Parse(daydata.Date);
                            return (from == null || date >= DateTime.Parse(newFrom)) && (to == null || date <= DateTime.Parse(to));
                        }).Take(30).ToList();


                        var returns = new List<ReturnDto>();

                        // Calculate Returns
                        for (int i = 1; i < filteredDayData.Count; i++)
                        {
                            var dailyReturn = Calculate.DailyReturn(filteredDayData[i].Close, filteredDayData[i - 1].Close);

                            returns.Add(new ReturnDto
                            {
                                date = filteredDayData[i].Date,
                                decimalReturn = dailyReturn,
                                percentileReturn = (dailyReturn * 100).ToString("F3")
                            });
                        }

                        if (returns.Count != 0)
                        {
                            return Ok(new { returns });
                        }
                        else
                        {
                            return StatusCode(404, new { error = "No stock data available for the specified date range" });
                        }
                    }
                    else
                    {
                        // Request failed, handle error
                        return StatusCode((int)response.StatusCode, new { error = $"Request failed: {response.ReasonPhrase}" });
                    }
                }
                catch(Exception ex) 
                {
                    return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
                }
            }
        }
    }
}
