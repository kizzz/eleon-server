using AutoMapper;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys;
using Volo.Abp.AutoMapper;
using VPortal.Identity.Module.Entities;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module;
public class TenantManagementDomainAutoMapperProfile : Profile
{
  public TenantManagementDomainAutoMapperProfile()
  {
    CreateMap<ModuleSettingsGotMsg, ModuleSettingsRefreshedMsg>();

    CreateMap<ApiKeyEntity, IdentityApiKeyEto>();
    CreateMap<ApiKeyEntity, IdentityApiKeyDto>();

  }
}
