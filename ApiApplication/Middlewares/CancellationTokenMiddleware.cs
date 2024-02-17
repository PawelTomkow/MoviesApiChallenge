using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Middlewares
{
    public class CancellationTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CancellationTokenMiddleware> _logger;

        public CancellationTokenMiddleware(RequestDelegate next, ILogger<CancellationTokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if cancellation has been requested
            if (context.RequestAborted.IsCancellationRequested)
            {
                var statusCode = StatusCodes.Status400BadRequest;
                context.Response.StatusCode = statusCode;
                var errorResponse = new ErrorResponse()
                {
                    StatusCode = statusCode,
                    Message = "Request canceled."

                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                
                _logger.LogInformation("Request: {Request} was canceled by user.", context);
                return;
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}