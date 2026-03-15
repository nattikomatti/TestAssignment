using Pre_register.Models.Requests;
using Pre_register.Models.Responses;

namespace Pre_register.Services.Interfaces;

public interface IVisitorService
{
    VisitorResponse PreRegister(PreRegisterRequest request);
    List<VisitorResponse> GetAll();
    VisitorResponse? GetById(Guid id);
}
