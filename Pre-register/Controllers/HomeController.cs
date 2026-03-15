using Microsoft.AspNetCore.Mvc;

namespace Pre_register.Controllers;

[ApiController]
[Route("api")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(new
        {
            System = "ระบบลงทะเบียนผู้มาติดต่อ + เปิดไม้กั้นอัตโนมัติ",
            Endpoints = new
            {
                PreRegister = "POST /api/visitor/pre-register",
                GetAllVisitors = "GET /api/visitor",
                GetVisitorById = "GET /api/visitor/{id}",
                VerifyByPlate = "POST /api/gate/verify-plate",
                VerifyByQr = "POST /api/gate/verify-qr?token={token}",
                GetGateLogs = "GET /api/gate/logs"
            }
        });
    }
}
