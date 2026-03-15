using Pre_register.Data;
using Pre_register.Repositories.Interfaces;

namespace Pre_register.Repositories;

public class VisitorRepository(AppDbContext dbContext) : IVisitorRepository
{
    public List<Visitor> GetAll() => dbContext.Visitors;

    public Visitor? GetById(Guid id) =>
        dbContext.Visitors.FirstOrDefault(v => v.Id == id);

    public Visitor? GetByPlateNumber(string plateNumber) =>
        dbContext.Visitors.FirstOrDefault(v =>
            v.PlateNumber.Equals(plateNumber, StringComparison.OrdinalIgnoreCase)
            && v.Status == VisitorStatus.Approved
            && v.ExpiryDate >= DateTime.Today);

    public Visitor? GetByToken(string token) =>
        dbContext.Visitors.FirstOrDefault(v =>
            v.Token == token
            && v.Status == VisitorStatus.Approved
            && v.ExpiryDate >= DateTime.Today);

    public Visitor Add(Visitor visitor)
    {
        visitor.Id = Guid.NewGuid();
        visitor.CreatedAt = DateTime.UtcNow;
        dbContext.Visitors.Add(visitor);
        return visitor;
    }

    public Visitor? Update(Visitor visitor)
    {
        var existing = GetById(visitor.Id);
        if (existing is null) return null;

        existing.FullName = visitor.FullName;
        existing.Company = visitor.Company;
        existing.PlateNumber = visitor.PlateNumber;
        existing.Purpose = visitor.Purpose;
        existing.ContactPerson = visitor.ContactPerson;
        existing.Status = visitor.Status;
        return existing;
    }
}
