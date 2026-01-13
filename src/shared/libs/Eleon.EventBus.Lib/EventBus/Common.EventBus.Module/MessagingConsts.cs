using Messaging.Module.Messages;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;

namespace Messaging.Module
{
  public static class MessagingConsts
  {
    public static readonly List<Type> SystemEventTypes = new List<Type>() // republish events from config
        {
            typeof(EntityDeletedEto<TenantEto>),
            typeof(EntityUpdatedEto<UserEto>),
            typeof(EntityCreatedEto<UserEto>),
            typeof(EntityDeletedEto<UserEto>),
            typeof(EntityUpdatedEto<IdentityRoleEto>),
            typeof(EntityCreatedEto<IdentityRoleEto>),
            typeof(EntityDeletedEto<IdentityRoleEto>),
            typeof(EntityCreatedEto<OrganizationUnitEto>),
            typeof(EntityUpdatedEto<OrganizationUnitEto>),
            typeof(EntityDeletedEto<OrganizationUnitEto>),
        };

    public static readonly List<string> EntityEtoTypes = new List<string>()
        {
            "ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Entities.EleoncoreTenantEntity",
            typeof(IdentityUserOrganizationUnit).FullName,
            typeof(OrganizationUnitRole).FullName,
            typeof(IdentityUserRole).FullName,
            typeof(PermissionGrant).FullName,
        };

    public static readonly List<Type> IgnoredMessageTypes = new List<Type>()
        {
            typeof(EntityCreatedEto<>),
            typeof(EntityUpdatedEto<>),
            typeof(EntityDeletedEto<>)
        };
  }
}
