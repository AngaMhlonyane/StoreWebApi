using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using StoreWebApi.Data;

namespace StoreWebApi.Middleware
{

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env; // Add environment dependency
        private const string ApiKeyHeaderName = "X-Api-Key";

        public ApiKeyMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Skip API key checks in Development
            if (_env.IsDevelopment())
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            var user = dbContext.Users.SingleOrDefault(u => u.ApiKey == extractedApiKey);
            if (user == null)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            // Add the user to the context for downstream use
            context.Items["User"] = user;

            await _next(context);
        }
    }
}