using System;
using Volo.Abp.Domain.Entities;

namespace VPortal.Identity.Module.Entities
{
  public class UserSessionStateEntity : AggregateRoot<Guid>
  {
    public Guid UserId { get; set; }
    public bool RequirePeriodicPasswordChange { get; set; }
    public bool PermissionErrorEncountered { get; set; }

    public UserSessionStateEntity(Guid id, Guid userId)
    {
      Id = id;
      UserId = userId;
    }
  }
}
