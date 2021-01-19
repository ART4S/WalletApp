using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.Exceptions;

namespace Web.Middlewares
{
    public class ExceptionHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    var logger = context.RequestServices
                        .GetService<ILogger<ExceptionHandlerMiddleware>>();

                    logger.LogError(ex, "");
                }
                else
                {
                    await HandleExceptionAsync(ex, context);
                }
            }
        }

        private static async Task HandleExceptionAsync(Exception exception, HttpContext context)
        {
            switch (exception)
            {
                case InvalidRequestException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { error = exception.Message });
                    return;

                case NotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(new { error = exception.Message });
                    return;

                default:
                    var logger = context.RequestServices
                        .GetService<ILogger<ExceptionHandlerMiddleware>>();

                    logger.LogError(exception, "Unexpected error");

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { error = "Something went wrong" });
                    return;
            }
        }
    }
}