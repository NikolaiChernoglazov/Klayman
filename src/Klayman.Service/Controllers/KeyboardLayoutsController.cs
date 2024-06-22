using FluentResults.Extensions.AspNetCore;
using Klayman.Application;
using Klayman.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Klayman.Service.Controllers;

[ApiController]
[Route("layouts")]
public class KeyboardLayoutsController(
    IKeyboardLayoutManager keyboardLayoutManager) : ControllerBase
{
    [HttpGet]
    [Route("current")]
    public ActionResult<KeyboardLayout> GetCurrentLayout()
    {
        return keyboardLayoutManager
            .GetCurrentLayout().ToActionResult();
    }
    
    [HttpGet]
    public ActionResult<List<KeyboardLayout>> GetCurrentLayouts()
    {
        return keyboardLayoutManager
            .GetCurrentLayouts().ToActionResult();
    }

    [HttpGet]
    [Route("all")]
    public ActionResult<List<KeyboardLayout>> GetAvailableLayouts(
        [FromQuery] string? query)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? keyboardLayoutManager.GetAllAvailableLayouts()
            : keyboardLayoutManager.GetAvailableLayoutsByQuery(query);
        return result.ToActionResult();
    }
    
    [HttpPost]
    public ActionResult<KeyboardLayout> AddLayout(KeyboardLayoutId layoutId)
    {
        if (!KeyboardLayoutId.IsValid(layoutId))
        {
            return NotValidKeyboardLayoutId(layoutId);
        }
        
        return keyboardLayoutManager
            .AddLayout(layoutId).ToActionResult();
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
        
        return keyboardLayoutManager
            .RemoveLayout(new KeyboardLayoutId(layoutId)).ToActionResult();
    }

    private BadRequestObjectResult NotValidKeyboardLayoutId(string layoutId)
        => BadRequest($"{layoutId} is not a valid KLID.");
}