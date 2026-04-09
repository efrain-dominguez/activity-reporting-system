using Serilog;
using System.Diagnostics;
using System.Security.Claims;

namespace ARS.API.Middleware
{

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var userName = GetUserIdentifier(context);

            try
            {
                await _next(context);

                sw.Stop();
                var statusCode = context.Response.StatusCode;
                var elapsedMs = sw.ElapsedMilliseconds;

                var logLevel = statusCode >= 500 ? LogLevel.Error :
                              statusCode >= 400 ? LogLevel.Warning :
                              LogLevel.Information;

                _logger.Log(logLevel,
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms - User: {UserName}",
                    requestMethod, requestPath, statusCode, elapsedMs, userName);

                // Warn on slow requests
                if (elapsedMs > 3000)
                {
                    _logger.LogWarning(
                        "SLOW REQUEST: {Method} {Path} took {ElapsedMs}ms - User: {UserName}",
                        requestMethod, requestPath, elapsedMs, userName);
                }
            }
            catch (Exception ex)
            {
                sw.Stop();

                _logger.LogError(ex,
                    "REQUEST FAILED: {Method} {Path} failed after {ElapsedMs}ms - User: {UserName}",
                    requestMethod, requestPath, sw.ElapsedMilliseconds, userName);

                throw;
            }
        }

        private string GetUserName(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
                return "Anonymous";

            // Try multiple claim types in order of preference
            return context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value
                ?? context.User.FindFirst("upn")?.Value
                ?? context.User.FindFirst("unique_name")?.Value
                ?? context.User.FindFirst("preferred_username")?.Value
                ?? context.User.FindFirst("email")?.Value
                ?? context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value
                ?? context.User.FindFirst("name")?.Value
                ?? context.User.FindFirst(ClaimTypes.Name)?.Value
                ?? context.User.FindFirst(ClaimTypes.Email)?.Value
                ?? context.User.Identity.Name
                ?? "Authenticated User";
        }

        private string GetUserIdentifier(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
                return "Anonymous";

            var email = GetUserName(context);
            var userId = context.User.FindFirst("oid")?.Value
                ?? context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var roles = string.Join(",", context.User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value));

            return $"{email} (ID: {userId?.Substring(0, 8)}..., Roles: {roles})";
        }
    }
}