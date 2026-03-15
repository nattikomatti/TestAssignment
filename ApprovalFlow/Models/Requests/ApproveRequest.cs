namespace ApprovalFlow.Models.Requests
{
    public class ApproveRequest
    {
        public string ApprovedBy { get; set; } = string.Empty;
        public string ApproverRole { get; set; } = string.Empty;
    }
}
