using System.Diagnostics;

namespace MealManagement.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";
        private const int MaxLength = 64;

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetOrCreate(context);

            context.TraceIdentifier = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    await _next(context);

                    _logger.LogInformation("{Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                        context.Request.Method, context.Request.Path,
                        context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
                }
                catch
                {
                    _logger.LogInformation("{Method} {Path} failed after {ElapsedMs} ms",
                        context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
        }

        private static string GetOrCreate(HttpContext context)
        {
            var incoming = context.Request.Headers[HeaderName].ToString();

            var isValid = !string.IsNullOrWhiteSpace(incoming)
                          && incoming.Length <= MaxLength
                          && incoming.All(c => char.IsLetterOrDigit(c) || c == '-');

            return isValid ? incoming : Guid.NewGuid().ToString();
        }
    }
}