using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Notificator.Module.Entities;

namespace VPortal.Notificator.Module.Repositories
{
  public interface IWebPushSubscriptionRepository : IBasicRepository<WebPushSubscriptionEntity>
  {
    Task<List<WebPushSubscriptionEntity>> GetByUsers(List<Guid> userIds);
  }
}
