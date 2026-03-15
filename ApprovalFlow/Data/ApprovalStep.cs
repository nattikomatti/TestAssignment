using ApprovalFlow.Workflow;

namespace ApprovalFlow.Data
{
    public class ApprovalStep
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RequestId { get; set; }
        public int StepOrder { get; set; }
        public string ApproverRole { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public ApprovalState State { get; set; } = ApprovalState.Pending;
        public DateTime? ApprovedAt { get; set; }
        public DateTime? UndoneAt { get; set; }
        public bool IsSentToExternalSystem { get; set; }
    }
}
