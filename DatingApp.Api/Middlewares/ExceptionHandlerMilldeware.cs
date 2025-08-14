using DatingApp.Api.Errors;
using System.Net;
using System.Text.Json;

namespace DatingApp.Api.Middlewares
{
    public class ExceptionHandlerMilldeware
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        private readonly ILoggerFactory _loggerFactory;

        private readonly RequestDelegate _next;


        public ExceptionHandlerMilldeware(IWebHostEnvironment hostEnvironment, ILoggerFactory loggerFactory = null, RequestDelegate next = null)
        {
            _hostEnvironment = hostEnvironment;
            _loggerFactory = loggerFactory;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("ExceptionHandlerMilldeware");
                logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _hostEnvironment.IsDevelopment() ?
                    new ApiException((int)HttpStatusCode.InternalServerError, ex?.Message, ex.StackTrace.ToString()) :
                    new ApiException((int)HttpStatusCode.InternalServerError, ex?.Message);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);

            }
        }



    }
}
