using Benefits.Application.Exceptions;
using Benefits.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = Argument.NotNull(logger, nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled Exception Occurred");

            var problemDetails = CreateProblemDetails(httpContext, exception);

            httpContext.Response.StatusCode = problemDetails.Status ?? 500;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

            return true;
        }

        private static ProblemDetails CreateProblemDetails(HttpContext httpContext, Exception exception)
        {
            var result = exception switch
            {
                NotFoundException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource Not Found",
                    Detail = ex.Message
                },
                BusinessRuleException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Business Rule Violation",
                    Detail = ex.Message
                },
                FluentValidation.ValidationException ex => CreateValidationProblemDetails(ex),
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred"
                }
            };

            result.Extensions["traceId"] = httpContext.TraceIdentifier;

            return result;
        }

        private static ValidationProblemDetails CreateValidationProblemDetails(FluentValidation.ValidationException ex)
        {
            var errors = ex.Errors.GroupBy(err => err.PropertyName).ToDictionary
                (
                    g => g.Key,
                    g => g.Select(validationFailure => validationFailure.ErrorMessage).ToArray()
                );

            return new ValidationProblemDetails(errors)
            {
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest
            };
        }
    }
}
