namespace Klayman.Domain.Results;

public abstract class ResultBase(
    OperationStatusCode statusCode,
    string? errorMessage = null,
    Exception? exception = null)
{
    public OperationStatusCode StatusCode { get; } = statusCode;

    public string? ErrorMessage { get; } = errorMessage;

    public Exception? Exception { get; } = exception;

    public bool IsSuccess => ErrorMessage is null;

    public bool IsFailed => ErrorMessage is not null;
}