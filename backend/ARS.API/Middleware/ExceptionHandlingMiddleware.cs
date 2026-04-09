using System.Net;
using System.Text.Json;

namespace ARS.API.Middleware
{

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);  // Continue to next middleware
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.Forbidden, "Access denied");
            }
            catch (ArgumentException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Invalid request");
            }
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, "Resource not found");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

       

        private async Task HandleExceptionAsync(
           HttpContext context,
           Exception ex,
           HttpStatusCode statusCode,
           string userMessage = "An error occurred processing your request")
        {
            // Extract user properly (like in RequestLoggingMiddleware)
            var userName = context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value
                ?? context.User?.FindFirst("upn")?.Value
                ?? context.User?.Identity?.Name
                ?? "Anonymous";

            // Log the exception with full details
            _logger.LogError(ex,
                "EXCEPTION: {StatusCode} {Method} {Path} - User: {UserName} - Message: {Message}",
                (int)statusCode,
                context.Request.Method,
                context.Request.Path,
                userName,
                ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = new
                {
                    //message = _env.IsDevelopment()
                    //    ? ex.Message  // Show details in development
                    //    : "An error occurred processing your request",  // Hide in production
                    message = _env.IsDevelopment() ? ex.Message : userMessage,
                    statusCode = (int)statusCode,
                    requestId = context.TraceIdentifier,
                    path = context.Request.Path.ToString(),  // ← Add this
                    timestamp = DateTime.UtcNow,  // ← Add this
                                                  // Only include stack trace in development
                    stackTrace = _env.IsDevelopment() ? ex.StackTrace : null
                }
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true  // ← Makes JSON easier to read
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}