using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Notificator.Module.Entities;

namespace VPortal.Notificator.Module.Repositories
{
  public interface INotificationsRepository : IBasicRepository<NotificationEntity>
  {
    Task<List<NotificationEntity>> GetActiveListAsync();
  }
}
