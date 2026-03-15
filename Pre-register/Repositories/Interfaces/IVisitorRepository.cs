using Pre_register.Data;

namespace Pre_register.Repositories.Interfaces;

public interface IVisitorRepository
{
    List<Visitor> GetAll();
    Visitor? GetById(Guid id);
    Visitor? GetByPlateNumber(string plateNumber);
    Visitor? GetByToken(string token);
    Visitor Add(Visitor visitor);
    Visitor? Update(Visitor visitor);
}
