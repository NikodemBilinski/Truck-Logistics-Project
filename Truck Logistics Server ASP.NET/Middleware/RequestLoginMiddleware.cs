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
            
            if(context.Request.Path.StartsWithSegments("/api/Auth/Login"))
            {
                if(context.Request.Method != HttpMethods.Post)
                {
                    context.Response.StatusCode = 405; // Method Not Allowed
                    return;
                }
                
                if(!context.Request.HasJsonContentType())
                {
                    context.Response.StatusCode = 415; // Unsupported Media Type
                    return;
                    
                }

                if (context.Connection.RemoteIpAddress != null)
                {
                    _logger.LogInformation($"=== REQUESTLOGINMIDDLEWARE Login attempt from IP: {context.Connection.RemoteIpAddress} ===");
                }


                var stopwatch = Stopwatch.StartNew();
                
                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation($"=== REQUESTLOGINMIDDLEWARE Login Request time took: {stopwatch.ElapsedMilliseconds} ms ===");

                return;
            }

            await _next(context);
        }
    }
}
