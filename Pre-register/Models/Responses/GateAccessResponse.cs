namespace Pre_register.Models.Responses;

public class GateAccessResponse
{
    public bool IsAllowed { get; set; }
    public string Message { get; set; } = string.Empty;
    public string VisitorName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
