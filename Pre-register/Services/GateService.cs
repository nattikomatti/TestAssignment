using Pre_register.Data;
using Pre_register.Models.Requests;
using Pre_register.Models.Responses;
using Pre_register.Repositories.Interfaces;
using Pre_register.Services.Interfaces;

namespace Pre_register.Services;

public class GateService(
    IVisitorRepository visitorRepository,
    IGateLogRepository gateLogRepository) : IGateService
{
    public GateAccessResponse VerifyByPlate(VerifyPlateRequest request)
    {
        var visitor = visitorRepository.GetByPlateNumber(request.PlateNumber);

        if (visitor is null)
        {
            LogGateAccess(null, request.PlateNumber, AccessMethod.LPR, false);
            return new GateAccessResponse
            {
                IsAllowed = false,
                Message = "ไม่พบข้อมูลการลงทะเบียนสำหรับทะเบียนรถนี้"
            };
        }

        LogGateAccess(visitor, visitor.PlateNumber, AccessMethod.LPR, true);
        visitor.Status = VisitorStatus.CheckedIn;
        visitorRepository.Update(visitor);

        return new GateAccessResponse
        {
            IsAllowed = true,
            Message = "เปิดไม้กั้น - ยินดีต้อนรับ",
            VisitorName = visitor.FullName,
            Company = visitor.Company,
            ContactPerson = visitor.ContactPerson
        };
    }

    public GateAccessResponse VerifyByQr(string token)
    {
        var visitor = visitorRepository.GetByToken(token);

        if (visitor is null)
        {
            LogGateAccess(null, "", AccessMethod.QR, false);
            return new GateAccessResponse
            {
                IsAllowed = false,
                Message = "QR Code ไม่ถูกต้องหรือหมดอายุ"
            };
        }

        LogGateAccess(visitor, visitor.PlateNumber, AccessMethod.QR, true);
        visitor.Status = VisitorStatus.CheckedIn;
        visitorRepository.Update(visitor);

        return new GateAccessResponse
        {
            IsAllowed = true,
            Message = "เปิดไม้กั้น - ยินดีต้อนรับ",
            VisitorName = visitor.FullName,
            Company = visitor.Company,
            ContactPerson = visitor.ContactPerson
        };
    }

    private void LogGateAccess(Visitor? visitor, string plateNumber, AccessMethod method, bool isSuccess)
    {
        gateLogRepository.Add(new GateLog
        {
            VisitorId = visitor?.Id ?? Guid.Empty,
            PlateNumber = plateNumber,
            Action = GateAction.Entry,
            Method = method,
            IsSuccess = isSuccess,
            Reason = isSuccess ? "อนุญาตเข้า" : "ไม่พบข้อมูลการลงทะเบียน"
        });
    }
}
