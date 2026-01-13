using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.Lifecycle.Feature.Module.Reporting;

namespace VPortal.Lifecycle.Feature.Module.Specifications
{
  public class ActorSpecification<T>
      : Specification<T>
      where T : LifecycleReport
  {
    private readonly IEnumerable<string> roles;
    private readonly string userRefId;
    private readonly Guid userId;

    public ActorSpecification(Guid userId, IEnumerable<string> roles)
    {
      this.roles = roles;
      this.userRefId = userId.ToString();
      this.userId = userId;
    }

    public override Expression<Func<T, bool>> ToExpression()
        => (report) =>
            (report.ActorType == LifecycleActorTypes.User && report.ActorRefId == userRefId)
            || (report.ActorType == LifecycleActorTypes.Initiator && report.LifecycleInitiatorId == userId)
            || (report.ActorType == LifecycleActorTypes.Role && roles.Contains(report.ActorRefId));
  }
}
