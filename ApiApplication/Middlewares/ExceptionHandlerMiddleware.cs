using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "Unknown exception.";


            switch (exception)
            {
                case ResourceNotFoundException resourceNotFoundException:
                    _logger.LogInformation("{DomainType} with {PropertyName}: {propertyValue} not found.", resourceNotFoundException.DomainType, resourceNotFoundException.PropertyName, resourceNotFoundException.PropertyValue);
                    statusCode = StatusCodes.Status404NotFound;
                    message = $"{resourceNotFoundException.DomainType.Name} with {resourceNotFoundException.PropertyName}: {resourceNotFoundException.PropertyValue} not found.";
                    break;
                default:
                    _logger.LogError("Unhandled exception. Please look into logs and investigate. Exception: {exception} HttpContext: {HttpContext}", exception, context);
                    break;
            } 

            var errorResponse = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message
            };
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}