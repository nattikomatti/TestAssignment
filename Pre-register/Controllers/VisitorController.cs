using Microsoft.AspNetCore.Mvc;
using Pre_register.Models.Requests;
using Pre_register.Services.Interfaces;

namespace Pre_register.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitorController(IVisitorService visitorService) : ControllerBase
{
    [HttpPost("pre-register")]
    public IActionResult PreRegister([FromBody] PreRegisterRequest request)
    {
        var result = visitorService.PreRegister(request);
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = visitorService.GetAll();
        return Ok(result);
    }

   
}
