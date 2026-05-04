using Microsoft.AspNetCore.Mvc;

namespace jargonz.api.Common.Results;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess) return Microsoft.AspNetCore.Http.Results.Ok();

        return result.Error.Code switch
        {
            var code when code.Contains("NotFound") => Microsoft.AspNetCore.Http.Results.NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error.Message,
                Status = 404
            }),
            var code when code.StartsWith("Validation.") => Microsoft.AspNetCore.Http.Results.BadRequest(
                new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = 400
                }),
            var code when code.StartsWith("Conflict.") => Microsoft.AspNetCore.Http.Results.Conflict(new ProblemDetails
            {
                Title = "Conflict",
                Detail = result.Error.Message,
                Status = 409
            }),
            var code when code.Contains("Unauthorized") => Microsoft.AspNetCore.Http.Results.Unauthorized(),
            var code when code.Contains("Forbidden") => Microsoft.AspNetCore.Http.Results.Forbid(),
            _ => Microsoft.AspNetCore.Http.Results.Problem(result.Error.Message, statusCode: 500)
        };
    }

    /// <summary>
    ///     Converts a Result<T> to an IResult for HTTP responses
    /// </summary>
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess) return Microsoft.AspNetCore.Http.Results.Ok(result.Value);

        return result.Error.Code switch
        {
            var code when code.Contains("NotFound") => Microsoft.AspNetCore.Http.Results.NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Error.Message,
                Status = 404
            }),
            var code when code.StartsWith("Validation.") => Microsoft.AspNetCore.Http.Results.BadRequest(
                new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = 400
                }),
            var code when code.StartsWith("Conflict.") => Microsoft.AspNetCore.Http.Results.Conflict(new ProblemDetails
            {
                Title = "Conflict",
                Detail = result.Error.Message,
                Status = 409
            }),
            var code when code.Contains("Unauthorized") => Microsoft.AspNetCore.Http.Results.Unauthorized(),
            var code when code.Contains("Forbidden") => Microsoft.AspNetCore.Http.Results.Forbid(),
            _ => Microsoft.AspNetCore.Http.Results.Problem(result.Error.Message, statusCode: 500)
        };
    }

    /// <summary>
    ///     Converts a Result<T> to an IResult with Created status for HTTP responses
    /// </summary>
    public static IResult ToCreatedHttpResult<T>(this Result<T> result, string uri)
    {
        if (result.IsSuccess) return Microsoft.AspNetCore.Http.Results.Created(uri, result.Value);

        return result.ToHttpResult();
    }

    /// <summary>
    ///     Maps a successful result to a new value
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result.Success(mapper(result.Value))
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    ///     Binds a result to another operation that returns a result
    /// </summary>
    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> func)
    {
        return result.IsSuccess
            ? await func(result.Value)
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    ///     Matches the result to execute different actions based on success or failure
    /// </summary>
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error);
    }
}
