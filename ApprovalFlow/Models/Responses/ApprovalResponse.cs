namespace ApprovalFlow.Models.Responses
{
    public class ApprovalResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ApprovalDetailResponse? Data { get; set; }
    }

    public class ApprovalDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string OverallState { get; set; } = string.Empty;
        public List<StepDetailResponse> Steps { get; set; } = [];
    }

    public class StepDetailResponse
    {
        public Guid Id { get; set; }
        public int StepOrder { get; set; }
        public string ApproverRole { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsSentToExternalSystem { get; set; }
        public bool CanUndo { get; set; }
    }

    public class HistoryResponse
    {
        public Guid Id { get; set; }
        public Guid StepId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public string? Reason { get; set; }
        public string PreviousState { get; set; } = string.Empty;
        public string NewState { get; set; } = string.Empty;
        public bool? ExternalSystemRollbackRequired { get; set; }
        public bool? ExternalSystemRollbackSuccess { get; set; }
    }
}
