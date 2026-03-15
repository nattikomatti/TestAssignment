namespace ApprovalFlow.Models.Requests
{
    public class CreateApprovalRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
    }
}
