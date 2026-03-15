namespace Pre_register.Data;

public class GateLog
{
    public Guid Id { get; set; }
    public Guid VisitorId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public GateAction Action { get; set; }
    public AccessMethod Method { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public enum GateAction
{
    Entry,
    Exit
}

public enum AccessMethod
{
    LPR,
    QR
}
