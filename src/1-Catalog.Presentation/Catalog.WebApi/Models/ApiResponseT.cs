using System.Text.Json.Serialization;

namespace Catalog.PublicApi.Models;

public sealed class ApiResponse<T> : ApiResponse
{
    [JsonConstructor]
    public ApiResponse(T result, bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)
        : base(success, successMessage, statusCode, errors)
    {
        Result = result;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ApiResponse()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public T Result { get; private init; }

    public static ApiResponse<T> Ok(T result) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result };

    public static ApiResponse<T> Ok(T result, string successMessage) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result, SuccessMessage = successMessage };
}