using Klayman.Application.KeyboardLayoutManagement;
using Klayman.Domain;
using Klayman.Service.ResponseBuilding;
using Microsoft.AspNetCore.Mvc;

namespace Klayman.Service.Controllers;

[ApiController]
[Route("layouts")]
public class KeyboardLayoutsController(
    IResponseBuilder responseBuilder,
    IKeyboardLayoutManager keyboardLayoutManager) : ControllerBase
{
    [HttpGet]
    [Route("current")]
    public ActionResult<KeyboardLayout> GetCurrentLayout()
    {
        var result = keyboardLayoutManager.GetCurrentLayout();
        return responseBuilder.Build(result);
    }
    
    [HttpGet]
    public ActionResult<List<KeyboardLayout>> GetCurrentLayouts()
    {
        var result = keyboardLayoutManager.GetCurrentLayouts();
        return responseBuilder.Build(result);
    }

    [HttpGet]
    [Route("all")]
    public ActionResult<List<KeyboardLayout>> GetAvailableLayouts(
        [FromQuery] string? query)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? keyboardLayoutManager.GetAllAvailableLayouts()
            : keyboardLayoutManager.GetAvailableLayoutsByQuery(query);
        return responseBuilder.Build(result);
    }
    
    [HttpPost]
    public ActionResult<KeyboardLayout> AddLayout(string layoutId)
    {
        if (!KeyboardLayoutId.IsValid(layoutId))
        {
            return NotValidKeyboardLayoutId(layoutId);
        }

        var result = keyboardLayoutManager.AddLayout(
            new KeyboardLayoutId(layoutId));
        return responseBuilder.Build(result);
    }
    
    [HttpDelete]
    [Route("{layoutId}")]
    public ActionResult<KeyboardLayout> RemoveLayout([FromRoute]
        string layoutId)
    {
        if (!KeyboardLayoutId.IsValid(layoutId))
        {
            return NotValidKeyboardLayoutId(layoutId);
        }
        
        var result = keyboardLayoutManager.RemoveLayout(new KeyboardLayoutId(layoutId));
        return responseBuilder.Build(result);
    }

    private BadRequestObjectResult NotValidKeyboardLayoutId(string layoutId)
        => BadRequest($"{layoutId} is not a valid KLID.");
}