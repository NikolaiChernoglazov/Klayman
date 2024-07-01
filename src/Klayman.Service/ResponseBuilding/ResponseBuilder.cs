using System.Net;
using Klayman.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace Klayman.Service.ResponseBuilding;

public class ResponseBuilder : IResponseBuilder
{
    private static readonly Dictionary<OperationStatusCode, HttpStatusCode>
        _statusCodeMapping = new()
        {
            { OperationStatusCode.Ok, HttpStatusCode.OK },
            { OperationStatusCode.AlreadyExists, HttpStatusCode.Conflict },
            { OperationStatusCode.NotFound, HttpStatusCode.NotFound },
            { OperationStatusCode.PermissionRequired, HttpStatusCode.InternalServerError },
            { OperationStatusCode.SystemFunctionFailed, HttpStatusCode.InternalServerError },
            { OperationStatusCode.UnknownError, HttpStatusCode.InternalServerError }
        };

    public ActionResult Build(Result result)
    {
        return result.IsSuccess
            ? new OkResult()
            : BuildErrorResponse(result);
    }

    public ActionResult<TValue> Build<TValue>(Result<TValue> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : BuildErrorResponse(result);
    }

    private static ObjectResult BuildErrorResponse(ResultBase result)
    {
        return new ObjectResult(
         new
         {
             Error = result.ErrorMessage
         })
        {
            StatusCode = (int)_statusCodeMapping[result.StatusCode]
        };
    }
}