namespace TrucksLogisticsServerAPI.Middleware
{
    public class GlobalRequestHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalRequestHandlerMiddleware> _logger;

        public GlobalRequestHandlerMiddleware(RequestDelegate next, ILogger<GlobalRequestHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/api/Auth/Login"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                
                var username = context.User.Identity.Name ?? "No_Username";

                var role = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "No_Role";

                var lastname = context.User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? "No_LastName";

                var ipaddress = context.Connection.RemoteIpAddress?.ToString() ?? "No_IP_Address";

                _logger.LogInformation($"=== GLOBALREQUESTHANDLERMIDDLEWARE REQUESTED User: {username}, Role: {role}, LastName: {lastname}, IP: {ipaddress} ===");

                
            }
            else
            {
                context.Response.StatusCode = 401; // Unauthorized
                _logger.LogWarning("=== GLOBALREQUESTHANDLERMIDDLEWARE REQUESTED (UNAUTHENTICATED) ===");
                return;
            }

            await _next(context);
        }
    }
}
