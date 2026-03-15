using ApprovalFlow.Models.Requests;
using ApprovalFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalController(IApprovalService approvalService) : ControllerBase
    {
        ///  ดูคำขออนุมัติทั้งหมด 
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(approvalService.GetAll());
        }

        /// ดูคำขออนุมัติตาม ID
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            var result = approvalService.GetById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// สร้างคำขออนุมัติใหม่
        [HttpPost]
        public IActionResult Create([FromBody] CreateApprovalRequest request)
        {
            var result = approvalService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        /// อนุมัติขั้นตอนถัดไป (Manager  Director   Finance)
        [HttpPost("{id:guid}/approve")]
        public IActionResult Approve(Guid id, [FromBody] ApproveRequest request)
        {
            var result = approvalService.Approve(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        ///  ยกเลิกการอนุมัติ (Undo) พร้อม rollback ระบบภายนอกถ้าจำเป็น 
        [HttpPost("{id:guid}/undo")]
        public IActionResult UndoApproval(Guid id, [FromBody] UndoApproveRequest request)
        {
            var result = approvalService.UndoApproval(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        ///  ดูประวัติการดำเนินการ (Audit Trail) 
        [HttpGet("{id:guid}/history")]
        public IActionResult GetHistory(Guid id)
        {
            return Ok(approvalService.GetHistory(id));
        }
    }
}
