using ApprovalFlow.Workflow;

namespace ApprovalFlow.Data
{
    public class ApprovalHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RequestId { get; set; }
        public Guid StepId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
        public ApprovalState PreviousState { get; set; }
        public ApprovalState NewState { get; set; }
        public bool? ExternalSystemRollbackRequired { get; set; }
        public bool? ExternalSystemRollbackSuccess { get; set; }
    }
}
