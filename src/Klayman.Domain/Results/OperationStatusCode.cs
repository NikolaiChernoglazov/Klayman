namespace Klayman.Domain.Results;

public enum OperationStatusCode
{
    Ok,
    SystemFunctionFailed,
    PermissionRequired,
    NotFound,
    AlreadyExists,
    UnknownError
}