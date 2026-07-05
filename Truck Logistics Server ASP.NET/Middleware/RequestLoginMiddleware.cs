namespace TrucksLogisticsServerAPI.Middleware
{
    public class RequestLoginMiddleware
    {
        private readonly RequestDelegate _next;
        

        public RequestLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("An Unexpected error occured");
            }
        }
    }
}
