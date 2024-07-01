using Klayman.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace Klayman.Service.ResponseBuilding;

public interface IResponseBuilder
{
    ActionResult Build(Result result);
    
    ActionResult<TValue> Build<TValue>(Result<TValue> result);
}