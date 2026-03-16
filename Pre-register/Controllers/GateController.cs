using Microsoft.AspNetCore.Mvc;
using Pre_register.Models.Requests;
using Pre_register.Repositories.Interfaces;
using Pre_register.Services.Interfaces;

namespace Pre_register.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GateController(IGateService gateService, IGateLogRepository gateLogRepository) : ControllerBase
{
    [HttpPost("verify-plate")]
    public IActionResult VerifyByPlate([FromBody] VerifyPlateRequest request)
    {
        var result = gateService.VerifyByPlate(request);
        return Ok(result);
    }

    [HttpPost("verify-qr")]
    public IActionResult VerifyByQr([FromQuery] string token)
    {
        var result = gateService.VerifyByQr(token);
        return Ok(result);
    }

    [HttpPost("exit-plate")]
    public IActionResult ExitByPlate([FromBody] VerifyPlateRequest request)
    {
        var result = gateService.ExitByPlate(request);
        return Ok(result);
    }

    [HttpPost("exit-qr")]
    public IActionResult ExitByQr([FromQuery] string token)
    {
        var result = gateService.ExitByQr(token);
        return Ok(result);
    }

    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        var logs = gateLogRepository.GetAll();
        return Ok(logs);
    }
}
