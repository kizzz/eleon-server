using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.GatewayManagement.Module.EntityFrameworkCore;

namespace EventBusManagement.Module.EntityFrameworkCore
{
  public class EventBusRepository : EfCoreRepository<GatewayManagementDbContext, EventBusEntity, Guid>, IEventBusRepository
  {
    public EventBusRepository(IDbContextProvider<GatewayManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
  }
}
