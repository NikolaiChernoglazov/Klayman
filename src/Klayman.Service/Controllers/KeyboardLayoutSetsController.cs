using Klayman.Application.KeyboardLayoutSetManagement;
using Klayman.Domain;
using Klayman.Service.ResponseBuilding;
using Microsoft.AspNetCore.Mvc;

namespace Klayman.Service.Controllers;

[ApiController]
[Route("layoutSets")]
public class KeyboardLayoutSetsController(
    IResponseBuilder responseBuilder,
    IKeyboardLayoutSetManager keyboardLayoutSetManager) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<KeyboardLayout>> GetCurrentLayoutSets()
    {
        return Ok(keyboardLayoutSetManager.GetLayoutSets());
    }
    
    [HttpPost]
    public ActionResult<KeyboardLayoutSet> AddLayoutSet(AddKeyboardLayoutSetRequest request)
    {
        var result = keyboardLayoutSetManager.AddLayoutSet(request.Name, request.LayoutIds);
        return responseBuilder.Build(result);
    }
    
    [HttpDelete]
    [Route("{name}")]
    public ActionResult RemoveLayoutSet([FromRoute] string name)
    {
        var result = keyboardLayoutSetManager.RemoveLayoutSet(name);
        return responseBuilder.Build(result);
    }

    [HttpOptions]
    [Route("{name}/apply")]
    public ActionResult ApplyLayoutSet([FromRoute] string name)
    {
        var result = keyboardLayoutSetManager.ApplyLayoutSet(name);
        return responseBuilder.Build(result);
    }
}