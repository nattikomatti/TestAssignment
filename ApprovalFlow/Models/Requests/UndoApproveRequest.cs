namespace ApprovalFlow.Models.Requests
{
    public class UndoApproveRequest
    {
        public string RequestedBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public Guid? StepId { get; set; }
    }
}
