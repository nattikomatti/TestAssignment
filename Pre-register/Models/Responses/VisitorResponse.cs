namespace Pre_register.Models.Responses;

public class VisitorResponse
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
    public string Status { get; set; } = string.Empty;
}
