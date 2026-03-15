using ApprovalFlow.Data;

namespace ApprovalFlow.Repositories.Interfaces
{
    public interface IApprovalHistoryRepository
    {
        void Add(ApprovalHistory history);
        List<ApprovalHistory> GetByRequestId(Guid requestId);
    }
}
