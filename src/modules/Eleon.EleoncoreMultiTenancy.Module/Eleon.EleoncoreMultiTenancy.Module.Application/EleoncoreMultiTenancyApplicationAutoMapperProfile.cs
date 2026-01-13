using AutoMapper;
using TenantSettings.Module.Cache;
using TenantSettings.Module.Models;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using VPortal.TenantManagement.Module.Tenants;
using SharedModels = TenantSettings.Module.Models;

namespace VPortal.TenantManagement.Module;

public class EleoncoreMultiTenancyApplicationAutoMapperProfile : Profile
{
  public EleoncoreMultiTenancyApplicationAutoMapperProfile()
  {
    CreateMap<TenantConnectionString, TenantConnectionStringDto>()
        .ReverseMap();

    CreateMap<Tenant, CommonTenantDto>(MemberList.None)
        .ReverseMap();


    CreateMap<Tenant, CommonTenantExtendedDto>()
        .Ignore(c => c.IsRoot)
        .Ignore(c => c.ParentId);
  }
}
