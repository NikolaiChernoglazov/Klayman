using FluentResults.Extensions.AspNetCore;
using Klayman.Application.KeyboardLayoutSetManagement;
using Klayman.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Klayman.Service.Controllers;

[ApiController]
[Route("layoutSets")]
public class KeyboardLayoutSetsController(
    IKeyboardLayoutSetManager keyboardLayoutSetManager) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<KeyboardLayout>> GetCurrentLayoutSets()
    {
        return Ok(keyboardLayoutSetManager.GetLayoutSets());
    }
    
    [HttpPost]
    public ActionResult<KeyboardLayout> AddLayoutSet(AddKeyboardLayoutSetRequest request)
    {
        return keyboardLayoutSetManager
            .AddLayoutSet(request.Name, request.LayoutIds).ToActionResult();
    }
    
    [HttpDelete]
    [Route("{name}")]
    public ActionResult RemoveLayoutSet([FromRoute] string name)
    {
        return keyboardLayoutSetManager.RemoveLayoutSet(name).ToActionResult();
    }

    [HttpOptions]
    [Route("{name}/apply")]
    public ActionResult ApplyLayoutSet([FromRoute] string name)
    {
        return keyboardLayoutSetManager.ApplyLayoutSet(name).ToActionResult();
    }
}