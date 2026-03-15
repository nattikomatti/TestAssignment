using ApprovalFlow.Data;
using ApprovalFlow.Repositories.Interfaces;

namespace ApprovalFlow.Repositories
{
    public class ApprovalHistoryRepository(AppDbContext db) : IApprovalHistoryRepository
    {
        public void Add(ApprovalHistory history) => db.ApprovalHistories.Add(history);

        public List<ApprovalHistory> GetByRequestId(Guid requestId) =>
            db.ApprovalHistories.Where(h => h.RequestId == requestId).OrderBy(h => h.PerformedAt).ToList();
    }
}
