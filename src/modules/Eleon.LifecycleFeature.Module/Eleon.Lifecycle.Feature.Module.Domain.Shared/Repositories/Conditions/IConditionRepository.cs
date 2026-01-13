using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;

namespace VPortal.Lifecycle.Feature.Module.Repositories.Conditions
{
  public interface IConditionRepository : IBasicRepository<ConditionEntity, Guid>
  {
    public Task<ConditionEntity> GetCondition(LifecycleConditionTargetType lifecycleConditionTargetType, Guid refId);
  }
}
