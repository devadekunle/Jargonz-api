using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace jargonz.api.Common;

/// <summary>
///     Global exception filter that converts FluentValidation exceptions to ProblemDetails responses
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Define the default response details
        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "Server Error";
        var extensions = new Dictionary<string, object?>();

        // Handle FluentValidation specifically
        if (exception is ValidationException validationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            title = "Validation Error";

            // Format FluentValidation errors into a dictionary
            var errors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            extensions.Add("errors", errors);
            httpContext.Response.StatusCode = statusCode;
        }

        // Use the built-in ProblemDetailsService to write the response
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions = extensions
            }

            // Metadata for logs and client-side debugging
        });
    }
}
