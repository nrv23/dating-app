using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware>  _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
    // el metodo se debe llamar InvokeAsync para que .net sepa que es un middleware
        public async Task InvokeAsync(HttpContext context) {
            try
            {
                // el context http , es la peticion entrante con toda la informacion

                await _next(context); // ejecutar de forma asincrona el siguiente middleware
            }
            catch (Exception ex)
            {
                // aqui se va manejar la execpion de forma global
                
                // log del error
                _logger.LogError(ex, ex.Message);
                // se setea la respuesta del middleware
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment() 
                    ? new ApiException(context.Response.StatusCode,ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode,ex.Message, "Internal Server Error");

               // generar un json de respuesta 
               var options = new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
               var json = JsonSerializer.Serialize(response,options);
               // respuesta 
               await context.Response.WriteAsync(json);
            }
        }
    }
}