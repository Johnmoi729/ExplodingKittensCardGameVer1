// Add this extension method to keep the code clean
using ExplodingKittens.API.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ExplodingKittens.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}