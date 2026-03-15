using ApprovalFlow.Models.Requests;
using ApprovalFlow.Models.Responses;

namespace ApprovalFlow.Services.Interfaces
{
    public interface IApprovalService
    {
        ApprovalResponse Create(CreateApprovalRequest request);
        ApprovalResponse Approve(Guid requestId, ApproveRequest request);
        ApprovalResponse UndoApproval(Guid requestId, UndoApproveRequest request);
        ApprovalResponse GetById(Guid requestId);
        List<ApprovalDetailResponse> GetAll();
        List<HistoryResponse> GetHistory(Guid requestId);
    }
}
