using ApprovalFlow.Data;
using ApprovalFlow.Repositories.Interfaces;

namespace ApprovalFlow.Repositories
{
    public class ApprovalRepository(AppDbContext db) : IApprovalRepository
    {
        public ApprovalRequest? GetById(Guid id) => db.ApprovalRequests.FirstOrDefault(r => r.Id == id);

        public List<ApprovalRequest> GetAll() => db.ApprovalRequests;

        public void Add(ApprovalRequest request) => db.ApprovalRequests.Add(request);
    }
}
