using Pre_register.Data;
using Pre_register.Infrastructure;
using Pre_register.Models.Requests;
using Pre_register.Models.Responses;
using Pre_register.Repositories.Interfaces;
using Pre_register.Services.Interfaces;
using Pre_register.Utils;

namespace Pre_register.Services;

public class VisitorService(
    IVisitorRepository visitorRepository,
    TokenGenerator tokenGenerator,
    QRCodeGenerator qrCodeGenerator) : IVisitorService
{
    public VisitorResponse PreRegister(PreRegisterRequest request)
    {
        var token = tokenGenerator.Generate();
        var qrData = qrCodeGenerator.Generate(token);

        var visitor = new Visitor
        {
            FullName = request.FullName,
            Company = request.Company,
            PlateNumber = request.PlateNumber,
            Purpose = request.Purpose,
            ContactPerson = request.ContactPerson,
            Token = token,
            QrCodeData = qrData,
            VisitDate = request.VisitDate,
            ExpiryDate = request.VisitDate.AddDays(1),
            Status = VisitorStatus.Approved
        };

        var created = visitorRepository.Add(visitor);
        return MapToResponse(created);
    }

    public List<VisitorResponse> GetAll() =>
        visitorRepository.GetAll().Select(MapToResponse).ToList();

    public VisitorResponse? GetById(Guid id)
    {
        var visitor = visitorRepository.GetById(id);
        return visitor is null ? null : MapToResponse(visitor);
    }

    private static VisitorResponse MapToResponse(Visitor visitor) => new()
    {
        Id = visitor.Id,
        FullName = visitor.FullName,
        Company = visitor.Company,
        PlateNumber = visitor.PlateNumber,
        Purpose = visitor.Purpose,
        ContactPerson = visitor.ContactPerson,
        Token = visitor.Token,
        QrCodeData = visitor.QrCodeData,
        VisitDate = visitor.VisitDate,
        Status = visitor.Status.ToString()
    };
}
