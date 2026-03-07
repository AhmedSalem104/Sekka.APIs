namespace Sekka.Core.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T value) { IsSuccess = true; Value = value; }
    private Result(Error error) { IsSuccess = false; Error = error; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
    public static Result<T> NotFound(string message) => new(new Error("NOT_FOUND", message));
    public static Result<T> BadRequest(string message) => new(new Error("BAD_REQUEST", message));
    public static Result<T> Conflict(string message) => new(new Error("CONFLICT", message));
    public static Result<T> Unauthorized(string message) => new(new Error("UNAUTHORIZED", message));
}

public record Error(string Code, string Message);
