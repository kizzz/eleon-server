using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;

namespace VPortal.Lifecycle.Feature.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface ILifecycleFeatureDbContext : IEfCoreDbContext
{
  DbSet<StatesGroupAuditEntity> StatesGroupAudits { get; set; }
  DbSet<StatesGroupTemplateEntity> StatesGroupTemplates { get; set; }
  DbSet<ConditionEntity> Conditions { get; set; }
}
