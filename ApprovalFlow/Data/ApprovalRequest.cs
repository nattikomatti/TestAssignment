using ApprovalFlow.Workflow;

namespace ApprovalFlow.Data
{
    public class ApprovalRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ApprovalState OverallState { get; set; } = ApprovalState.Pending;
        public List<ApprovalStep> Steps { get; set; } = [];
    }
}
