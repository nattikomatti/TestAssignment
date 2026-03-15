namespace Pre_register.Models.Requests;

public class PreRegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
}
