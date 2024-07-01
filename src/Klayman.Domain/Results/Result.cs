namespace Klayman.Domain.Results;

public class Result : ResultBase
{
    private Result(OperationStatusCode statusCode, string? errorMessage = null, Exception? exception = null)
        : base(statusCode, errorMessage, exception)
    {
    }

    public static Result Ok()
    {
        return new Result(OperationStatusCode.Ok);
    }
    
    public static Result Fail(OperationStatusCode statusCode,
        string errorMessage, Exception? exception = null)
    {
        return new Result(statusCode, errorMessage, exception);
    }
    
    public static Result Fail(
        string errorMessage, Exception? exception = null)
    {
        return new Result(OperationStatusCode.UnknownError, errorMessage, exception);
    }
    
    public static Result SystemFunctionFailed(string errorMessage)
    {
        return Fail(OperationStatusCode.SystemFunctionFailed, errorMessage);
    }
    
    public static Result PermissionRequired(string errorMessage)
    {
        return Fail(OperationStatusCode.PermissionRequired, errorMessage);
    }
    
    public static Result NotFound(string errorMessage)
    {
        return Fail(OperationStatusCode.PermissionRequired, errorMessage);
    }
    
    public static Result AlreadyExists(string errorMessage)
    {
        return Fail(OperationStatusCode.AlreadyExists, errorMessage);
    }
    
    public static Result<TValue> Ok<TValue>(TValue value)
    {
        return Result<TValue>.Ok(value);
    }
}

public class Result<TValue> : ResultBase
{
    public TValue Value { get; } = default!;

    private Result(TValue value, OperationStatusCode statusCode)
        : base(statusCode)
    {
        Value = value;
    }

    private Result(OperationStatusCode statusCode,
        string? errorMessage = null, Exception? exception = null)
        : base(statusCode, errorMessage, exception)
    {
    }

    public static implicit operator Result<TValue>(Result result)
    {
        return new Result<TValue>(result.StatusCode, result.ErrorMessage, result.Exception);
    }
    
    public static implicit operator Result(Result<TValue> result)
    {
        return result.IsSuccess
            ? Result.Ok()
            : Result.Fail(result.StatusCode, result.ErrorMessage!, result.Exception);
    }

    public static Result<TValue> Ok(TValue value)
    {
        return new Result<TValue>(value, OperationStatusCode.Ok);
    }
    
    public Result<TValue> Map(Func<TValue, TValue> mapper)
    {
        return IsFailed ? this : new Result<TValue>(mapper(Value!), StatusCode);
    }
    
    public Result<TOtherValue> Map<TOtherValue>(Func<TValue, TOtherValue> mapper)
    {
        return IsFailed
            ? new Result<TOtherValue>(StatusCode, ErrorMessage, Exception)
            : new Result<TOtherValue>(mapper(Value!), StatusCode);
    }

    public Result WithUpdatedErrorMessage(Func<string, string> errorMessageUpdater)
    {
        return Result.Fail(StatusCode, errorMessageUpdater(ErrorMessage!), Exception);
    }
}
