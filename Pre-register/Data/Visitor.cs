namespace Pre_register.Data;

public class Visitor
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string QrCodeData { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public VisitorStatus Status { get; set; } = VisitorStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum VisitorStatus
{
    Pending,
    Approved,
    CheckedIn,
    CheckedOut,
    Expired
}
