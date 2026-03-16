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
            LogGateAccess(null, request.PlateNumber, AccessMethod.LPR, GateAction.Entry, false, "ไม่พบข้อมูลการลงทะเบียน");
            return new GateAccessResponse
            {
                IsAllowed = false,
                Message = "ไม่พบข้อมูลการลงทะเบียนสำหรับทะเบียนรถนี้"
            };
        }

        LogGateAccess(visitor, visitor.PlateNumber, AccessMethod.LPR, GateAction.Entry, true, "อนุญาตเข้า");
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
            LogGateAccess(null, "", AccessMethod.QR, GateAction.Entry, false, "ไม่พบข้อมูลการลงทะเบียน");
            return new GateAccessResponse
            {
                IsAllowed = false,
                Message = "QR Code ไม่ถูกต้องหรือหมดอายุ"
            };
        }

        LogGateAccess(visitor, visitor.PlateNumber, AccessMethod.QR, GateAction.Entry, true, "อนุญาตเข้า");
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

    public GateAccessResponse ExitByPlate(VerifyPlateRequest request)
    {
        var visitor = visitorRepository.GetCheckedInByPlateNumber(request.PlateNumber);

        if (visitor is null)
        {
            LogGateAccess(null, request.PlateNumber, AccessMethod.LPR, GateAction.Exit, false, "ไม่พบข้อมูลการลงทะเบียน");
            return new GateAccessResponse
            {
                IsAllowed = false,
                Message = "ไม่พบข้อมูลการลงทะเบียนสำหรับทะเบียนรถนี้"
            };
        }

        LogGateAccess(visitor, visitor.PlateNumber, AccessMethod.LPR, GateAction.Exit, true, "อนุญาตออก");
        visitor.Status = VisitorStatus.CheckedOut;
        visitorRepository.Update(visitor);

        return new GateAccessResponse
        {
            IsAllowed = true,
            Message = "เปิดไม้กั้น - ขอบคุณที่มาเยี่ยมชม",
            VisitorName = visitor.FullName,
            Company = visitor.Company,
            ContactPerson = visitor.ContactPerson
        };
    }

    public GateAccessResponse ExitByQr(string token)
    {
        var visitor = visitorRepository.GetCheckedInByToken(token);

        if (visitor is null)
        {
            LogGateAccess(null, "", AccessMethod.QR, GateAction.Exit, false, "ไม่พบข้อมูลการลงทะเบียน");
            return new GateAccessResponse
            {
                IsAllowed = false,
                Message = "QR Code ไม่ถูกต้องหรือยังไม่ได้เช็คอินเข้า"
            };
        }

        LogGateAccess(visitor, visitor.PlateNumber, AccessMethod.QR, GateAction.Exit, true, "อนุญาตออก");
        visitor.Status = VisitorStatus.CheckedOut;
        visitorRepository.Update(visitor);

        return new GateAccessResponse
        {
            IsAllowed = true,
            Message = "เปิดไม้กั้น - ขอบคุณที่มาเยี่ยมชม",
            VisitorName = visitor.FullName,
            Company = visitor.Company,
            ContactPerson = visitor.ContactPerson
        };
    }

    private void LogGateAccess(Visitor? visitor, string plateNumber, AccessMethod method, GateAction action, bool isSuccess, string reason)
    {
        gateLogRepository.Add(new GateLog
        {
            VisitorId = visitor?.Id ?? Guid.Empty,
            PlateNumber = plateNumber,
            Action = action,
            Method = method,
            IsSuccess = isSuccess,
            Reason = reason
        });
    }
}
