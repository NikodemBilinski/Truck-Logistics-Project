using System.Diagnostics;

namespace TrucksLogisticsServerAPI.Middleware
{
    public class RequestLoginMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public RequestLoginMiddleware(RequestDelegate next, ILogger<RequestLoginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            if(context.Request.Path.StartsWithSegments("api/login"))
            {
                if(context.Request.Method != HttpMethods.Post)
                {
                    context.Response.StatusCode = 405; // Method Not Allowed
                    return;
                }
                if(context.Connection.RemoteIpAddress != null)
                {
                    _logger.LogInformation($"Login attempt from IP: {context.Connection.RemoteIpAddress}");
                }
                if(context.Request.ContentType != "application/json")
                {
                    context.Response.StatusCode = 415; // Unsupported Media Type
                    return;
                }

                _logger.LogInformation($"Login Request time took: {stopwatch.ElapsedMilliseconds} ms");
                _logger.LogInformation("Login request is valid. Proceeding to next middleware.");
                await _next(context);
            }
        }
    }
}
