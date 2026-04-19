namespace Sekka.Core.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T value) { IsSuccess = true; Value = value; }
    private Result(Error error) { IsSuccess = false; Error = error; }
    private Result(Error error, T value) { IsSuccess = false; Error = error; Value = value; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
    public static Result<T> NotFound(string message) => new(new Error("NOT_FOUND", message));
    public static Result<T> BadRequest(string message) => new(new Error("BAD_REQUEST", message));
    public static Result<T> BadRequest(string message, T value) => new(new Error("BAD_REQUEST", message), value);
    public static Result<T> Conflict(string message) => new(new Error("CONFLICT", message));
    public static Result<T> Unauthorized(string message) => new(new Error("UNAUTHORIZED", message));
    public static Result<T> NotImplemented(string message) => new(new Error("NOT_IMPLEMENTED", message));
}

public record Error(string Code, string Message);
