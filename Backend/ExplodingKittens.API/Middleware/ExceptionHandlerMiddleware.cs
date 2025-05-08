using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ExplodingKittens.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExplodingKittens.API.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions globally
    /// </summary>
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred";

            if (exception is NotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exception is InvalidGameStateException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                message = "You are not authorized to perform this action";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
        }
    }
}