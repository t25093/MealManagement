using MealManagement.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // הלקוח סגר את החיבור באמצע. אין למי להחזיר תשובה, וזה לא באג
                _logger.LogInformation("Request {Path} was cancelled by the client", context.Request.Path);
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception ex)
        {
            var (status, title, level) = ex switch
            {
                BusinessRuleException => (StatusCodes.Status400BadRequest, "Bad Request", LogLevel.Warning),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", LogLevel.Warning),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden", LogLevel.Warning),
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found", LogLevel.Information),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict", LogLevel.Warning),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", LogLevel.Error)
            };

            var correlationId = context.TraceIdentifier;

            if (level == LogLevel.Error)
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}. CorrelationId: {CorrelationId}",
                    context.Request.Method, context.Request.Path, correlationId);
            else
                _logger.Log(level, "{ExceptionType} on {Method} {Path}: {Message}. CorrelationId: {CorrelationId}",
                    ex.GetType().Name, context.Request.Method, context.Request.Path, ex.Message, correlationId);

            // אם חלק מהתשובה כבר נשלח, אי אפשר לשנות סטטוס. זורקים הלאה
            if (context.Response.HasStarted)
                throw ex;

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "אירעה שגיאה בלתי צפויה. ניתן לפנות לתמיכה עם מזהה הבקשה."
                    : ex.Message,
                Instance = context.Request.Path
            };
            problem.Extensions["correlationId"] = correlationId;

            context.Response.Clear();
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }
}