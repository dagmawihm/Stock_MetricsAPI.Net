using Newtonsoft.Json;
using System.Globalization;

namespace StockMetricsAPI.Middlewares
{
    public class ValidateInputMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateInputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var from = context.Request.Query["from"].ToString();
            var to = context.Request.Query["to"].ToString();

            if (!context.Request.Path.StartsWithSegments("/swagger"))
            {

                // Check if symbol is provided
                if (!context.Request.Query.ContainsKey("symbol"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Stock Ticker Symbol is required." }, Formatting.Indented));
                    return;
                }

                // Check if from date is provided
                if (!string.IsNullOrEmpty(from))
                {
                    // Check if from date were valid (YYYY-MM-DD)
                    if (!DateValidator.IsDateFormatValid(from))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Invalid date format on from date! use (YYYY-MM-DD)." }, Formatting.Indented));
                        return;
                    }

                    // Check if from date is not in the future
                    if (DateValidator.ConvertStringToDate(from) > DateTime.Today)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Dates cannot be in the future. from Date is in the future!"}, Formatting.Indented));
                        return;
                    }
                }

                // Check if to date is provided
                if (!string.IsNullOrEmpty(to))
                {
                    // Check if to date were valid (YYYY-MM-DD)
                    if (!DateValidator.IsDateFormatValid(to))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Invalid date format on to date! use (YYYY-MM-DD)."}, Formatting.Indented));
                        return;
                    }

                    // Check if to date is not in the future
                    if (DateValidator.ConvertStringToDate(to) > DateTime.Today)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Dates cannot be in the future. to Date is in the future!"}, Formatting.Indented));
                        return;
                    }
                }

                // Check if both from and to dates are provided
                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                {
                    // Ensure from date is before to date
                    if (DateValidator.ConvertStringToDate(from) > DateValidator.ConvertStringToDate(to))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Invalid date range: from date must be before to date."}, Formatting.Indented));
                        return;
                    }

                    // Check if the difference between from and to dates is not more than 30 days
                    TimeSpan difference = DateValidator.ConvertStringToDate(to) - DateValidator.ConvertStringToDate(from);
                    if (difference.TotalDays > 30)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Date range exceeds 30 days."}, Formatting.Indented));
                        return;
                    }

                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }


        public class DateValidator
        {
            public static bool IsDateFormatValid(string dateString)
            {
                DateTime result;
                return DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }

            public static DateTime ConvertStringToDate(string dateString)
            {
                DateTime result;
                DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                return result;
            }
        }

    }
}
