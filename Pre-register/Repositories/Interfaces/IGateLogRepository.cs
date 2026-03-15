using Pre_register.Data;

namespace Pre_register.Repositories.Interfaces;

public interface IGateLogRepository
{
    List<GateLog> GetAll();
    List<GateLog> GetByVisitorId(Guid visitorId);
    GateLog Add(GateLog gateLog);
}
