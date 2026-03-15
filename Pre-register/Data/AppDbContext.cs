namespace Pre_register.Data;

public class AppDbContext
{
    private static readonly List<Visitor> _visitors = [];
    private static readonly List<GateLog> _gateLogs = [];

    static AppDbContext()
    {
        _visitors.AddRange([
            new Visitor
            {
                Id = Guid.NewGuid(),
                FullName = "สมชาย ใจดี",
                Company = "บริษัท ABC จำกัด",
                PlateNumber = "กข 1234",
                Purpose = "ประชุม",
                ContactPerson = "สมหญิง รักดี",
                Token = "TOKEN-001",
                QrCodeData = "PRE-REG:TOKEN-001",
                VisitDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddDays(1),
                Status = VisitorStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Visitor
            {
                Id = Guid.NewGuid(),
                FullName = "สมหมาย รักเรียน",
                Company = "บริษัท XYZ จำกัด",
                PlateNumber = "คง 5678",
                Purpose = "ส่งเอกสาร",
                ContactPerson = "วิชัย สร้างสรรค์",
                Token = "TOKEN-002",
                QrCodeData = "PRE-REG:TOKEN-002",
                VisitDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddDays(1),
                Status = VisitorStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Visitor
            {
                Id = Guid.NewGuid(),
                FullName = "ทดสอบ หมดอายุ",
                Company = "บริษัท Test จำกัด",
                PlateNumber = "จฉ 9999",
                Purpose = "ทดสอบ",
                ContactPerson = "ผู้ดูแล",
                Token = "TOKEN-003",
                QrCodeData = "PRE-REG:TOKEN-003",
                VisitDate = DateTime.Today.AddDays(-3),
                ExpiryDate = DateTime.Today.AddDays(-2),
                Status = VisitorStatus.Expired,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        ]);
    }

    public List<Visitor> Visitors => _visitors;
    public List<GateLog> GateLogs => _gateLogs;
}
