using Pre_register.Data;
using Pre_register.Repositories.Interfaces;

namespace Pre_register.Repositories;

public class GateLogRepository(AppDbContext dbContext) : IGateLogRepository
{
    public List<GateLog> GetAll() => dbContext.GateLogs;

    public List<GateLog> GetByVisitorId(Guid visitorId) =>
        dbContext.GateLogs.Where(g => g.VisitorId == visitorId).ToList();

    public GateLog Add(GateLog gateLog)
    {
        gateLog.Id = Guid.NewGuid();
        gateLog.Timestamp = DateTime.UtcNow;
        dbContext.GateLogs.Add(gateLog);
        return gateLog;
    }
}
