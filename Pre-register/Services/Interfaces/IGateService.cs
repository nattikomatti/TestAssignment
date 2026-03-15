using Pre_register.Models.Requests;
using Pre_register.Models.Responses;

namespace Pre_register.Services.Interfaces;

public interface IGateService
{
    GateAccessResponse VerifyByPlate(VerifyPlateRequest request);
    GateAccessResponse VerifyByQr(string token);
}
