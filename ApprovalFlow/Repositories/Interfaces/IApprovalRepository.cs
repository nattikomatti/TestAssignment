using ApprovalFlow.Data;

namespace ApprovalFlow.Repositories.Interfaces
{
    public interface IApprovalRepository
    {
        ApprovalRequest? GetById(Guid id);
        List<ApprovalRequest> GetAll();
        void Add(ApprovalRequest request);
    }
}
